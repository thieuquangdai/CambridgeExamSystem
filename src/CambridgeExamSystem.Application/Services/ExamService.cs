using AutoMapper;
using CambridgeExamSystem.Application.Common;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Application.Services;

public sealed class ExamService(
    IExamRepository examRepository,
    ITestAttemptRepository attemptRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    TimeProvider timeProvider) : IExamService
{
    public async Task<List<ExamLevelDto>> GetLevelsAsync(CancellationToken cancellationToken = default)
    {
        var levels = await examRepository.GetLevelsAsync(cancellationToken);
        return levels.Select(mapper.Map<ExamLevelDto>).ToList();
    }

    public async Task<List<ExamPaperSummaryDto>> GetPublishedExamsAsync(string? levelCode, int? userId, CancellationToken cancellationToken = default)
    {
        var papers = await examRepository.GetPapersAsync(levelCode, publishedOnly: true, cancellationToken);
        var summaries = papers.Select(mapper.Map<ExamPaperSummaryDto>).ToList();

        if (userId is int id)
        {
            var attempts = await attemptRepository.GetUserAttemptsAsync(id, null, cancellationToken);
            foreach (var summary in summaries)
            {
                ApplyUserAttempts(summary, attempts.Where(a => a.ExamPaperId == summary.ExamPaperId));
            }
        }

        return summaries;
    }

    public async Task<ExamPaperDetailDto?> GetExamDetailAsync(int examPaperId, int? userId, CancellationToken cancellationToken = default)
    {
        var paper = await examRepository.GetPaperWithContentAsync(examPaperId, cancellationToken);
        if (paper is null || !paper.IsPublished || !paper.IsActive)
        {
            return null;
        }

        var detail = new ExamPaperDetailDto
        {
            Summary = mapper.Map<ExamPaperSummaryDto>(paper),
            Sections = paper.Sections.OrderBy(s => s.SectionOrder).Select(mapper.Map<SectionSummaryDto>).ToList()
        };

        if (userId is int id)
        {
            var attempts = await attemptRepository.GetUserAttemptsAsync(id, examPaperId, cancellationToken);
            ApplyUserAttempts(detail.Summary, attempts);
            detail.History = attempts.Where(a => a.IsSubmitted).Select(mapper.Map<AttemptHistoryItemDto>).ToList();
        }

        return detail;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var papers = await examRepository.GetPapersAsync(null, publishedOnly: false, cancellationToken);
        var totals = await attemptRepository.GetAttemptTotalsAsync(cancellationToken);
        return new AdminDashboardDto
        {
            TotalExams = papers.Count,
            PublishedExams = papers.Count(p => p.IsPublished),
            TotalQuestions = await examRepository.CountActiveQuestionsAsync(cancellationToken),
            TotalAttempts = totals.Total,
            SubmittedAttempts = totals.Submitted,
            AveragePercentage = totals.AveragePercentage
        };
    }

    public async Task<List<ExamPaperSummaryDto>> GetAllExamsAsync(CancellationToken cancellationToken = default)
    {
        var papers = await examRepository.GetPapersAsync(null, publishedOnly: false, cancellationToken);
        return papers.Select(mapper.Map<ExamPaperSummaryDto>).ToList();
    }

    public async Task<ExamPaperEditDto?> GetExamForEditAsync(int examPaperId, CancellationToken cancellationToken = default)
    {
        var paper = await examRepository.GetPaperAsync(examPaperId, cancellationToken);
        return paper is null ? null : mapper.Map<ExamPaperEditDto>(paper);
    }

    public async Task<ExamContentDto?> GetExamContentAsync(int examPaperId, CancellationToken cancellationToken = default)
    {
        var paper = await examRepository.GetPaperWithContentAsync(examPaperId, cancellationToken);
        if (paper is null)
        {
            return null;
        }

        return new ExamContentDto
        {
            Exam = mapper.Map<ExamPaperSummaryDto>(paper),
            Sections = paper.Sections.OrderBy(s => s.SectionOrder).Select(s => new SectionContentDto
            {
                ExamSectionId = s.ExamSectionId,
                SectionCode = s.SectionCode,
                SectionName = s.SectionName,
                SectionOrder = s.SectionOrder,
                MaxScore = s.MaxScore,
                Questions = s.Questions.Where(q => q.IsActive).OrderBy(q => q.QuestionOrder).Select(mapper.Map<QuestionAdminDto>).ToList()
            }).ToList()
        };
    }

    public async Task<List<QuestionTypeDto>> GetQuestionTypesAsync(CancellationToken cancellationToken = default)
    {
        var types = await examRepository.GetQuestionTypesAsync(cancellationToken);
        return types.Select(mapper.Map<QuestionTypeDto>).ToList();
    }

    public async Task<ServiceResult<int>> SaveExamAsync(ExamPaperEditDto dto, int? createdByUserId, CancellationToken cancellationToken = default)
    {
        var examCode = dto.ExamCode.Trim().ToUpperInvariant();
        if (await examRepository.GetLevelAsync(dto.LevelId, cancellationToken) is null)
        {
            return ServiceResult<int>.Failure("Cấp độ không tồn tại.");
        }

        if (await examRepository.ExamCodeExistsAsync(examCode, dto.ExamPaperId == 0 ? null : dto.ExamPaperId, cancellationToken))
        {
            return ServiceResult<int>.Failure($"Mã đề '{examCode}' đã tồn tại.");
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        ExamPaper paper;
        if (dto.ExamPaperId == 0)
        {
            paper = new ExamPaper { CreatedAtUtc = now, CreatedByUserId = createdByUserId };
            examRepository.AddPaper(paper);
        }
        else
        {
            paper = await examRepository.GetPaperAsync(dto.ExamPaperId, cancellationToken)
                ?? throw new NotFoundException("Không tìm thấy đề thi.");
            paper.UpdatedAtUtc = now;
            paper.VersionNo++;
        }

        paper.LevelId = dto.LevelId;
        paper.ExamCode = examCode;
        paper.ExamName = dto.ExamName.Trim();
        paper.Description = dto.Description?.Trim();
        paper.DurationMinutes = dto.DurationMinutes;
        paper.PassPercentage = dto.PassPercentage;
        paper.IsPublished = dto.IsPublished;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<int>.Success(paper.ExamPaperId);
    }

    public async Task<ServiceResult<bool>> SetPublishedAsync(int examPaperId, bool isPublished, CancellationToken cancellationToken = default)
    {
        var paper = await examRepository.GetPaperWithContentAsync(examPaperId, cancellationToken);
        if (paper is null)
        {
            return ServiceResult<bool>.Failure("Không tìm thấy đề thi.");
        }

        if (isPublished && !paper.Sections.SelectMany(s => s.Questions).Any(q => q.IsActive))
        {
            return ServiceResult<bool>.Failure("Đề thi phải có ít nhất một câu hỏi trước khi xuất bản.");
        }

        paper.IsPublished = isPublished;
        paper.UpdatedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(isPublished);
    }

    public async Task<ServiceResult<int>> AddSectionAsync(SectionEditDto dto, CancellationToken cancellationToken = default)
    {
        var paper = await examRepository.GetPaperWithContentAsync(dto.ExamPaperId, cancellationToken);
        if (paper is null)
        {
            return ServiceResult<int>.Failure("Không tìm thấy đề thi.");
        }

        var code = dto.SectionCode.Trim().ToUpperInvariant();
        if (paper.Sections.Any(s => s.SectionCode == code))
        {
            return ServiceResult<int>.Failure($"Đề thi đã có phần {code}.");
        }

        var section = new Section
        {
            ExamPaperId = paper.ExamPaperId,
            SectionCode = code,
            SectionName = dto.SectionName.Trim(),
            SectionDescription = dto.SectionDescription?.Trim(),
            DurationMinutes = dto.DurationMinutes,
            SectionOrder = paper.Sections.Select(s => s.SectionOrder).DefaultIfEmpty(0).Max() + 1
        };
        examRepository.AddSection(section);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<int>.Success(section.ExamSectionId);
    }

    public async Task<ServiceResult<int>> AddQuestionAsync(QuestionEditDto dto, CancellationToken cancellationToken = default)
    {
        var section = await examRepository.GetSectionWithQuestionsAsync(dto.ExamSectionId, cancellationToken);
        if (section is null)
        {
            return ServiceResult<int>.Failure("Không tìm thấy phần thi.");
        }

        var type = await examRepository.GetQuestionTypeAsync(dto.QuestionTypeId, cancellationToken);
        if (type is null)
        {
            return ServiceResult<int>.Failure("Dạng câu hỏi không hợp lệ.");
        }

        var options = dto.Options.Where(o => !string.IsNullOrWhiteSpace(o.OptionText)).ToList();
        var errors = ValidateQuestionForType(type, dto, options);
        if (errors.Count > 0)
        {
            return ServiceResult<int>.Failure(errors);
        }

        var question = new Question
        {
            ExamSectionId = section.ExamSectionId,
            QuestionTypeId = type.QuestionTypeId,
            QuestionCode = string.IsNullOrWhiteSpace(dto.QuestionCode) ? null : dto.QuestionCode.Trim(),
            QuestionText = dto.QuestionText.Trim(),
            InstructionText = dto.InstructionText?.Trim(),
            QuestionOrder = section.Questions.Select(q => q.QuestionOrder).DefaultIfEmpty(0).Max() + 1,
            Score = dto.Score,
            DifficultyCode = dto.DifficultyCode,
            CorrectTextAnswer = type.AllowsTextAnswer ? dto.CorrectTextAnswer?.Trim() : null
        };

        if (type.RequiresOptions)
        {
            for (var i = 0; i < options.Count; i++)
            {
                question.Options.Add(new AnswerOption
                {
                    OptionCode = ((char)('A' + i)).ToString(),
                    OptionText = options[i].OptionText!.Trim(),
                    OptionOrder = i + 1,
                    IsCorrect = options[i].IsCorrect,
                    MatchingKey = string.IsNullOrWhiteSpace(options[i].MatchingKey) ? null : options[i].MatchingKey!.Trim()
                });
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.ExplanationText))
        {
            question.Explanations.Add(new Explanation
            {
                ExplanationText = dto.ExplanationText.Trim(),
                TranscriptText = dto.TranscriptText?.Trim()
            });
        }

        AddMedia(question, MediaType.Image, dto.ImageUrl, 1);
        AddMedia(question, MediaType.Audio, dto.AudioUrl, 2);

        section.MaxScore += question.Score;
        section.ExamPaper.TotalScore += question.Score;
        section.ExamPaper.UpdatedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        examRepository.AddQuestion(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<int>.Success(question.QuestionId);
    }

    public async Task<ServiceResult<int>> DeactivateQuestionAsync(int questionId, CancellationToken cancellationToken = default)
    {
        var question = await examRepository.GetQuestionAsync(questionId, cancellationToken);
        if (question is null || !question.IsActive)
        {
            return ServiceResult<int>.Failure("Không tìm thấy câu hỏi.");
        }

        question.IsActive = false;
        question.Section.MaxScore = Math.Max(0, question.Section.MaxScore - question.Score);
        question.Section.ExamPaper.TotalScore = Math.Max(0, question.Section.ExamPaper.TotalScore - question.Score);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<int>.Success(question.Section.ExamPaperId);
    }

    private static List<string> ValidateQuestionForType(QuestionType type, QuestionEditDto dto, List<OptionEditDto> options)
    {
        var errors = new List<string>();
        if (type.AllowsTextAnswer && string.IsNullOrWhiteSpace(dto.CorrectTextAnswer))
        {
            errors.Add("Dạng câu hỏi này cần đáp án đúng dạng văn bản (có thể ngăn cách nhiều đáp án bằng '|').");
        }

        if (!type.RequiresOptions)
        {
            return errors;
        }

        if (options.Count < 2)
        {
            errors.Add("Cần ít nhất 2 lựa chọn.");
        }

        var correctCount = options.Count(o => o.IsCorrect);
        switch (type.TypeCode)
        {
            case QuestionTypeCodes.MultipleChoice:
            case QuestionTypeCodes.TrueFalse:
                if (correctCount != 1)
                {
                    errors.Add("Câu hỏi một đáp án phải có đúng 1 lựa chọn đúng.");
                }

                break;
            case QuestionTypeCodes.MultipleSelect:
                if (correctCount < 1)
                {
                    errors.Add("Cần ít nhất 1 lựa chọn đúng.");
                }

                break;
            case QuestionTypeCodes.Matching:
                if (options.Any(o => string.IsNullOrWhiteSpace(o.MatchingKey)))
                {
                    errors.Add("Mỗi lựa chọn của câu nối cần có MatchingKey.");
                }

                break;
        }

        return errors;
    }

    private static void AddMedia(Question question, MediaType mediaType, string? url, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        question.Media.Add(new QuestionMedia
        {
            MediaType = mediaType,
            FileUrl = url.Trim(),
            FileName = Path.GetFileName(url.Trim()),
            DisplayOrder = displayOrder
        });
    }

    private static void ApplyUserAttempts(ExamPaperSummaryDto summary, IEnumerable<TestAttempt> attempts)
    {
        var list = attempts.ToList();
        summary.AttemptCount = list.Count(a => a.IsSubmitted);
        summary.BestPercentage = list.Where(a => a.IsSubmitted).Max(a => a.PercentageScore);
        summary.ActiveAttemptId = list.FirstOrDefault(a => a.IsOpen)?.ExamAttemptId;
    }
}
