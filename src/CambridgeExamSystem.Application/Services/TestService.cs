using System.Text.Json;
using AutoMapper;
using CambridgeExamSystem.Application.Common;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Application.Services;

public sealed class TestService(
    IExamRepository examRepository,
    ITestAttemptRepository attemptRepository,
    IUserProgressRepository progressRepository,
    IAchievementService achievementService,
    IGradingService gradingService,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    TimeProvider timeProvider) : ITestService
{
    private const int ClockToleranceSeconds = 5;

    public async Task<long> StartOrResumeAsync(int userId, int examPaperId, CancellationToken cancellationToken = default)
    {
        var paper = await examRepository.GetPaperAsync(examPaperId, cancellationToken);
        if (paper is null || !paper.IsPublished || !paper.IsActive)
        {
            throw new NotFoundException("Đề thi không tồn tại hoặc chưa được xuất bản.");
        }

        var open = await attemptRepository.GetOpenAttemptAsync(userId, examPaperId, cancellationToken);
        if (open is not null)
        {
            return open.ExamAttemptId;
        }

        var now = UtcNow();
        var attempt = new TestAttempt
        {
            UserId = userId,
            ExamPaperId = examPaperId,
            AttemptNumber = await attemptRepository.CountAttemptsAsync(userId, examPaperId, cancellationToken) + 1,
            Status = AttemptStatus.InProgress,
            StartedAtUtc = now,
            LastSavedAtUtc = now
        };
        attempt.Events.Add(new AttemptEvent { EventType = AttemptEventTypes.Started, CreatedAtUtc = now });
        attemptRepository.Add(attempt);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return attempt.ExamAttemptId;
    }

    public async Task<TakeTestDto?> GetTakeTestAsync(long examAttemptId, int userId, CancellationToken cancellationToken = default)
    {
        var attempt = await attemptRepository.GetWithAnswersAsync(examAttemptId, userId, cancellationToken);
        if (attempt is null || !attempt.IsOpen)
        {
            return null;
        }

        var paper = await examRepository.GetPaperWithContentAsync(attempt.ExamPaperId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy đề thi.");

        if (RemainingSeconds(attempt, paper) <= 0)
        {
            await GradeAndCloseAsync(attempt, paper, AttemptStatus.Expired, AttemptEventTypes.TimeExpired, cancellationToken);
            return null;
        }

        var now = UtcNow();
        var isResumed = attempt.Status == AttemptStatus.Paused || attempt.Answers.Count > 0 || attempt.TotalSecondsUsed > 0;
        if (isResumed)
        {
            var eventType = attempt.Status == AttemptStatus.Paused ? AttemptEventTypes.Resumed : AttemptEventTypes.BrowserReconnected;
            attempt.Status = AttemptStatus.InProgress;
            attempt.LastSavedAtUtc = now;
            attemptRepository.AddEvent(new AttemptEvent { ExamAttemptId = attempt.ExamAttemptId, EventType = eventType, CreatedAtUtc = now });
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var dto = new TakeTestDto
        {
            ExamAttemptId = attempt.ExamAttemptId,
            ExamPaperId = paper.ExamPaperId,
            ExamName = paper.ExamName,
            LevelName = paper.Level.LevelName,
            DurationMinutes = paper.DurationMinutes,
            RemainingSeconds = RemainingSeconds(attempt, paper),
            CurrentQuestionIndex = attempt.CurrentQuestionIndex,
            LastSavedAtUtc = attempt.LastSavedAtUtc,
            IsResumed = isResumed,
            SavedAnswers = attempt.Answers.Select(mapper.Map<SavedAnswerDto>).ToList()
        };

        var index = 0;
        foreach (var section in paper.Sections.OrderBy(s => s.SectionOrder))
        {
            var sectionDto = new TestSectionDto
            {
                ExamSectionId = section.ExamSectionId,
                SectionCode = section.SectionCode,
                SectionName = section.SectionName,
                SectionDescription = section.SectionDescription,
                Groups = section.QuestionGroups
                    .Where(g => g.IsActive)
                    .OrderBy(g => g.GroupOrder)
                    .Select(mapper.Map<TestGroupDto>)
                    .ToList()
            };

            foreach (var question in ActiveQuestions(section))
            {
                var questionDto = mapper.Map<TestQuestionDto>(question);
                questionDto.Index = index++;
                questionDto.Options = OrderOptionsForDisplay(question, attempt.ExamAttemptId)
                    .Select(mapper.Map<TestOptionDto>)
                    .ToList();
                if (question.QuestionType.TypeCode == QuestionTypeCodes.Matching)
                {
                    questionDto.MatchingKeys = question.Options
                        .Where(o => !string.IsNullOrWhiteSpace(o.MatchingKey))
                        .Select(o => o.MatchingKey!.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Order(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                sectionDto.Questions.Add(questionDto);
            }

            dto.Sections.Add(sectionDto);
        }

        dto.CurrentQuestionIndex = Math.Clamp(dto.CurrentQuestionIndex, 0, Math.Max(0, dto.TotalQuestions - 1));
        return dto;
    }

    public async Task<SaveAnswerResult> SaveAnswerAsync(int userId, SaveAnswerRequest request, CancellationToken cancellationToken = default)
    {
        var attempt = await attemptRepository.GetWithAnswersAsync(request.ExamAttemptId, userId, cancellationToken);
        if (attempt is null || !attempt.IsOpen)
        {
            return new SaveAnswerResult { Success = false, IsExpired = attempt?.IsSubmitted == true, Message = "Phiên làm bài không hợp lệ hoặc đã nộp." };
        }

        var paper = await examRepository.GetPaperAsync(attempt.ExamPaperId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy đề thi.");

        var now = UtcNow();
        var seconds = ClampElapsedSeconds(attempt, request.TimeSpentSeconds, now);
        attempt.TotalSecondsUsed += seconds;

        if (RemainingSeconds(attempt, paper) <= 0)
        {
            var fullPaper = await examRepository.GetPaperWithContentAsync(attempt.ExamPaperId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy đề thi.");
            await GradeAndCloseAsync(attempt, fullPaper, AttemptStatus.Expired, AttemptEventTypes.TimeExpired, cancellationToken);
            return new SaveAnswerResult { Success = false, IsExpired = true, SavedAtUtc = now, Message = "Đã hết thời gian làm bài. Bài thi đã được nộp tự động." };
        }

        if (request.QuestionId is int questionId)
        {
            var question = await examRepository.GetQuestionAsync(questionId, cancellationToken);
            if (question is null || !question.IsActive || question.Section.ExamPaperId != attempt.ExamPaperId)
            {
                return new SaveAnswerResult { Success = false, Message = "Câu hỏi không thuộc đề thi này." };
            }

            if (request.SelectedOptionId is int optionId && question.Options.All(o => o.QuestionOptionId != optionId))
            {
                return new SaveAnswerResult { Success = false, Message = "Đáp án không hợp lệ." };
            }

            var answer = attempt.Answers.FirstOrDefault(a => a.QuestionId == questionId);
            if (answer is null)
            {
                answer = new TestAnswer { QuestionId = questionId };
                attempt.Answers.Add(answer);
            }

            answer.SelectedOptionId = request.SelectedOptionId;
            answer.TextAnswer = string.IsNullOrWhiteSpace(request.TextAnswer) ? null : request.TextAnswer.Trim();
            answer.AnswerJson = string.IsNullOrWhiteSpace(request.AnswerJson) ? null : request.AnswerJson;
            answer.IsAnswered = gradingService.IsAnswered(answer.SelectedOptionId, answer.TextAnswer, answer.AnswerJson);
            answer.TimeSpentSeconds += seconds;
            answer.SavedAtUtc = now;

            attemptRepository.AddEvent(new AttemptEvent
            {
                ExamAttemptId = attempt.ExamAttemptId,
                EventType = AttemptEventTypes.AnswerSaved,
                EventDataJson = JsonSerializer.Serialize(new { questionId }),
                CreatedAtUtc = now
            });
        }

        attempt.CurrentQuestionIndex = Math.Max(0, request.CurrentQuestionIndex);
        attempt.CurrentQuestionId = request.CurrentQuestionId ?? request.QuestionId ?? attempt.CurrentQuestionId;
        attempt.LastSavedAtUtc = now;
        attempt.Status = AttemptStatus.InProgress;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SaveAnswerResult { Success = true, SavedAtUtc = now, RemainingSeconds = RemainingSeconds(attempt, paper) };
    }

    public async Task PauseAsync(int userId, long examAttemptId, int currentQuestionIndex, int timeSpentSeconds, CancellationToken cancellationToken = default)
    {
        var attempt = await attemptRepository.GetWithAnswersAsync(examAttemptId, userId, cancellationToken);
        if (attempt is null || !attempt.IsOpen)
        {
            return;
        }

        var now = UtcNow();
        attempt.TotalSecondsUsed += ClampElapsedSeconds(attempt, timeSpentSeconds, now);
        attempt.CurrentQuestionIndex = Math.Max(0, currentQuestionIndex);
        attempt.Status = AttemptStatus.Paused;
        attempt.LastSavedAtUtc = now;
        attemptRepository.AddEvent(new AttemptEvent { ExamAttemptId = attempt.ExamAttemptId, EventType = AttemptEventTypes.Paused, CreatedAtUtc = now });
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<long> SubmitAsync(int userId, long examAttemptId, CancellationToken cancellationToken = default)
    {
        var attempt = await attemptRepository.GetWithAnswersAsync(examAttemptId, userId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy bài làm.");

        if (attempt.IsSubmitted)
        {
            return attempt.ExamAttemptId;
        }

        if (!attempt.IsOpen)
        {
            throw new BusinessRuleException("Bài làm này không thể nộp.");
        }

        var paper = await examRepository.GetPaperWithContentAsync(attempt.ExamPaperId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy đề thi.");

        await GradeAndCloseAsync(attempt, paper, AttemptStatus.Submitted, AttemptEventTypes.Submitted, cancellationToken);
        return attempt.ExamAttemptId;
    }

    public async Task<AttemptResultDto?> GetResultAsync(long examAttemptId, int userId, CancellationToken cancellationToken = default)
    {
        var attempt = await attemptRepository.GetWithAnswersAsync(examAttemptId, userId, cancellationToken);
        if (attempt is null || !attempt.IsSubmitted || attempt.Result is null)
        {
            return null;
        }

        var paper = await examRepository.GetPaperWithContentAsync(attempt.ExamPaperId, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy đề thi.");

        var result = attempt.Result;
        var dto = new AttemptResultDto
        {
            ExamAttemptId = attempt.ExamAttemptId,
            ExamPaperId = paper.ExamPaperId,
            ExamName = paper.ExamName,
            LevelName = paper.Level.LevelName,
            StartedAtUtc = attempt.StartedAtUtc,
            SubmittedAtUtc = attempt.SubmittedAtUtc,
            TotalSecondsUsed = attempt.TotalSecondsUsed,
            TotalScore = result.TotalScore,
            MaxScore = result.MaxScore,
            PercentageScore = result.PercentageScore,
            PassPercentage = paper.PassPercentage,
            CorrectCount = result.CorrectCount,
            WrongCount = result.WrongCount,
            UnansweredCount = result.UnansweredCount,
            StarsEarned = result.StarsEarned,
            ResultMessage = result.ResultMessage
        };

        var answers = attempt.Answers.ToDictionary(a => a.QuestionId);
        var index = 0;
        foreach (var section in paper.Sections.OrderBy(s => s.SectionOrder))
        {
            var sectionResult = new SectionResultDto { SectionName = section.SectionName };
            foreach (var question in section.Questions
                         .Where(q => q.IsActive || answers.ContainsKey(q.QuestionId))
                         .OrderBy(q => q.QuestionOrder))
            {
                answers.TryGetValue(question.QuestionId, out var answer);
                var explanation = question.Explanations.Where(e => e.IsPublished).OrderBy(e => e.ExplanationId).FirstOrDefault();
                var isCorrect = answer?.IsCorrect == true;
                var scoreEarned = answer?.ScoreEarned ?? 0;

                dto.Questions.Add(new QuestionResultDto
                {
                    QuestionId = question.QuestionId,
                    Index = ++index,
                    SectionName = section.SectionName,
                    QuestionText = question.QuestionText,
                    TypeCode = question.QuestionType.TypeCode,
                    UserAnswerText = gradingService.DescribeAnswer(question, answer),
                    CorrectAnswerText = gradingService.DescribeCorrectAnswer(question),
                    IsAnswered = answer?.IsAnswered == true,
                    IsCorrect = isCorrect,
                    ScoreEarned = scoreEarned,
                    MaxScore = question.Score,
                    ExplanationText = explanation?.ExplanationText,
                    TranscriptText = explanation?.TranscriptText,
                    GrammarNote = explanation?.GrammarNote,
                    VocabularyNote = explanation?.VocabularyNote
                });

                sectionResult.QuestionCount++;
                sectionResult.MaxScore += question.Score;
                sectionResult.Score += scoreEarned;
                sectionResult.CorrectCount += isCorrect ? 1 : 0;
            }

            dto.Sections.Add(sectionResult);
        }

        var earned = await progressRepository.GetAchievementsEarnedForAttemptAsync(
            userId, result.GradedAtUtc.AddSeconds(-1), result.GradedAtUtc.AddSeconds(1), cancellationToken);
        dto.NewAchievements = earned.Select(ua =>
        {
            var achievement = mapper.Map<AchievementDto>(ua.Achievement);
            achievement.IsEarned = true;
            achievement.EarnedAtUtc = ua.EarnedAtUtc;
            return achievement;
        }).ToList();

        return dto;
    }

    public async Task<List<AttemptHistoryItemDto>> GetHistoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        var attempts = await attemptRepository.GetUserAttemptsAsync(userId, null, cancellationToken);
        return attempts.Where(a => a.IsSubmitted).Select(mapper.Map<AttemptHistoryItemDto>).ToList();
    }

    public async Task<List<ActiveAttemptDto>> GetActiveAttemptsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var attempts = await attemptRepository.GetOpenAttemptsAsync(userId, cancellationToken);
        return attempts.Select(a => new ActiveAttemptDto
        {
            ExamAttemptId = a.ExamAttemptId,
            ExamPaperId = a.ExamPaperId,
            ExamName = a.ExamPaper.ExamName,
            LevelName = a.ExamPaper.Level.LevelName,
            LastSavedAtUtc = a.LastSavedAtUtc,
            AnsweredCount = a.Answers.Count(x => x.IsAnswered),
            RemainingSeconds = RemainingSeconds(a, a.ExamPaper)
        }).ToList();
    }

    private async Task GradeAndCloseAsync(TestAttempt attempt, ExamPaper paper, AttemptStatus finalStatus, string eventType, CancellationToken cancellationToken)
    {
        var now = UtcNow();
        var answers = attempt.Answers.ToDictionary(a => a.QuestionId);
        decimal totalScore = 0, maxScore = 0;
        int correct = 0, wrong = 0, unanswered = 0;

        foreach (var question in paper.Sections.SelectMany(ActiveQuestions))
        {
            answers.TryGetValue(question.QuestionId, out var answer);
            var outcome = gradingService.Grade(question, answer);
            maxScore += question.Score;
            totalScore += outcome.ScoreEarned;

            if (!outcome.IsAnswered)
            {
                unanswered++;
            }
            else if (outcome.IsCorrect)
            {
                correct++;
            }
            else
            {
                wrong++;
            }

            if (answer is not null)
            {
                answer.IsAnswered = outcome.IsAnswered;
                answer.IsCorrect = outcome.IsCorrect;
                answer.ScoreEarned = outcome.ScoreEarned;
            }
        }

        var percentage = maxScore == 0 ? 0 : Math.Round(totalScore * 100 / maxScore, 2);
        var passed = paper.PassPercentage is null || percentage >= paper.PassPercentage;

        attempt.Status = finalStatus;
        attempt.IsSubmitted = true;
        attempt.SubmittedAtUtc = now;
        attempt.LastSavedAtUtc = now;
        attempt.TotalScore = totalScore;
        attempt.MaxScore = maxScore;
        attempt.CorrectCount = correct;
        attempt.WrongCount = wrong;
        attempt.UnansweredCount = unanswered;
        attempt.PercentageScore = percentage;
        attemptRepository.AddEvent(new AttemptEvent { ExamAttemptId = attempt.ExamAttemptId, EventType = eventType, CreatedAtUtc = now });

        var statistics = await progressRepository.GetStatisticsAsync(attempt.UserId, cancellationToken);
        if (statistics is null)
        {
            statistics = new UserStatistic { UserId = attempt.UserId, UpdatedAtUtc = now };
            progressRepository.AddStatistics(statistics);
        }

        RewardCalculator.ApplyAttempt(statistics, percentage, passed, attempt.TotalSecondsUsed, DateOnly.FromDateTime(now), now);

        var examStars = RewardCalculator.CalculateExamStars(percentage);
        RewardCalculator.AddStars(statistics, examStars);
        progressRepository.AddStarTransaction(new StarTransaction
        {
            UserId = attempt.UserId,
            ExamAttemptId = attempt.ExamAttemptId,
            Amount = examStars,
            TransactionType = StarTransactionType.EXAM_COMPLETED,
            Description = $"Hoàn thành đề {paper.ExamCode} ({percentage:0.##}%)",
            CreatedAtUtc = now
        });

        var newAchievements = await achievementService.AwardEligibleAsync(statistics, attempt, cancellationToken);
        var message = await progressRepository.GetMotivationalMessageAsync(percentage, cancellationToken);

        attempt.Result = new AttemptResult
        {
            ExamAttemptId = attempt.ExamAttemptId,
            TotalScore = totalScore,
            MaxScore = maxScore,
            PercentageScore = percentage,
            CorrectCount = correct,
            WrongCount = wrong,
            UnansweredCount = unanswered,
            StarsEarned = examStars + newAchievements.Sum(a => a.RewardStars),
            ResultMessage = message?.MessageText,
            GradedAtUtc = now
        };

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Question> ActiveQuestions(Section section) =>
        section.Questions.Where(q => q.IsActive).OrderBy(q => q.QuestionOrder);

    private static IEnumerable<AnswerOption> OrderOptionsForDisplay(Question question, long examAttemptId)
    {
        var ordered = question.Options.OrderBy(o => o.OptionOrder).ToList();
        if (question.QuestionType.TypeCode != QuestionTypeCodes.Ordering || ordered.Count < 2)
        {
            return ordered;
        }

        var random = new Random(HashCode.Combine(examAttemptId, question.QuestionId));
        var shuffled = ordered.OrderBy(_ => random.Next()).ToList();
        if (shuffled.SequenceEqual(ordered))
        {
            shuffled.Reverse();
        }

        return shuffled;
    }

    private static int RemainingSeconds(TestAttempt attempt, ExamPaper paper) =>
        Math.Max(0, (paper.DurationMinutes * 60) - attempt.TotalSecondsUsed);

    private static int ClampElapsedSeconds(TestAttempt attempt, int requestedSeconds, DateTime nowUtc)
    {
        var wallClock = (int)Math.Ceiling((nowUtc - attempt.LastSavedAtUtc).TotalSeconds) + ClockToleranceSeconds;
        return Math.Clamp(requestedSeconds, 0, Math.Max(0, wallClock));
    }

    private DateTime UtcNow() => TruncateToSeconds(timeProvider.GetUtcNow().UtcDateTime);

    private static DateTime TruncateToSeconds(DateTime value) =>
        new(value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);
}
