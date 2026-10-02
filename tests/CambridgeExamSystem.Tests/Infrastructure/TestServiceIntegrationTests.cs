using System.Text.Json;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CambridgeExamSystem.Tests.Infrastructure;

public class TestServiceIntegrationTests(SqlServerDatabaseFixture fixture) : IClassFixture<SqlServerDatabaseFixture>
{
    private async Task<int> CreateStudentAsync()
    {
        await using var scope = fixture.Services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"student-{Guid.NewGuid():N}@test.local";
        var user = new User { UserName = email, Email = email, FullName = "Integration Student" };
        var created = await users.CreateAsync(user, "Passw0rd!1");
        Assert.True(created.Succeeded, string.Join("; ", created.Errors.Select(e => e.Description)));
        await users.AddToRoleAsync(user, RoleNames.Student);
        return user.Id;
    }

    private async Task<ExamPaper> LoadSampleExamAsync()
    {
        await using var scope = fixture.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CambridgeDbContext>();
        return await context.ExamPapers
            .AsNoTracking()
            .Include(e => e.Sections).ThenInclude(s => s.Questions).ThenInclude(q => q.Options)
            .Include(e => e.Sections).ThenInclude(s => s.Questions).ThenInclude(q => q.QuestionType)
            .SingleAsync(e => e.ExamCode == "KET-SAMPLE-01");
    }

    private static SaveAnswerRequest CorrectAnswer(long attemptId, Question question)
    {
        var request = new SaveAnswerRequest { ExamAttemptId = attemptId, QuestionId = question.QuestionId };
        var options = question.Options.OrderBy(o => o.OptionOrder).ToList();
        switch (question.QuestionType.TypeCode)
        {
            case QuestionTypeCodes.MultipleChoice:
            case QuestionTypeCodes.TrueFalse:
                request.SelectedOptionId = options.Single(o => o.IsCorrect).QuestionOptionId;
                break;
            case QuestionTypeCodes.MultipleSelect:
                request.AnswerJson = JsonSerializer.Serialize(new { selectedOptionIds = options.Where(o => o.IsCorrect).Select(o => o.QuestionOptionId) });
                break;
            case QuestionTypeCodes.Matching:
                request.AnswerJson = JsonSerializer.Serialize(new { matches = options.ToDictionary(o => o.QuestionOptionId.ToString(), o => o.MatchingKey) });
                break;
            case QuestionTypeCodes.Ordering:
                request.AnswerJson = JsonSerializer.Serialize(new { orderedOptionIds = options.Select(o => o.QuestionOptionId) });
                break;
            default:
                request.TextAnswer = question.CorrectTextAnswer!.Split('|')[0];
                break;
        }

        return request;
    }

    [SqlServerFact]
    public async Task Start_reuses_the_open_attempt_and_restores_saved_answers()
    {
        var userId = await CreateStudentAsync();
        var exam = await LoadSampleExamAsync();
        var question = exam.Sections.SelectMany(s => s.Questions).First(q => q.QuestionType.TypeCode == QuestionTypeCodes.MultipleChoice);

        await using var scope = fixture.Services.CreateAsyncScope();
        var tests = scope.ServiceProvider.GetRequiredService<ITestService>();
        var attemptId = await tests.StartOrResumeAsync(userId, exam.ExamPaperId);
        var saved = await tests.SaveAnswerAsync(userId, CorrectAnswer(attemptId, question));
        await tests.PauseAsync(userId, attemptId, currentQuestionIndex: 2, timeSpentSeconds: 5);

        Assert.True(saved.Success);
        Assert.Equal(attemptId, await tests.StartOrResumeAsync(userId, exam.ExamPaperId));
        var take = await tests.GetTakeTestAsync(attemptId, userId);
        Assert.NotNull(take);
        Assert.True(take.IsResumed);
        Assert.Equal(2, take.CurrentQuestionIndex);
        Assert.Contains(take.SavedAnswers, a => a.QuestionId == question.QuestionId && a.IsAnswered);
    }

    [SqlServerFact]
    public async Task Save_rejects_options_from_other_questions_and_other_users()
    {
        var userId = await CreateStudentAsync();
        var otherUserId = await CreateStudentAsync();
        var exam = await LoadSampleExamAsync();
        var questions = exam.Sections.SelectMany(s => s.Questions).Where(q => q.Options.Count > 0).Take(2).ToList();

        await using var scope = fixture.Services.CreateAsyncScope();
        var tests = scope.ServiceProvider.GetRequiredService<ITestService>();
        var attemptId = await tests.StartOrResumeAsync(userId, exam.ExamPaperId);

        var foreignOption = await tests.SaveAnswerAsync(userId, new SaveAnswerRequest
        {
            ExamAttemptId = attemptId,
            QuestionId = questions[0].QuestionId,
            SelectedOptionId = questions[1].Options.First().QuestionOptionId
        });
        Assert.False(foreignOption.Success);

        var otherUser = await tests.SaveAnswerAsync(otherUserId, CorrectAnswer(attemptId, questions[0]));
        Assert.False(otherUser.Success);
        Assert.Null(await tests.GetTakeTestAsync(attemptId, otherUserId));
    }

    [SqlServerFact]
    public async Task Submitting_all_correct_answers_scores_full_marks_and_awards_stars()
    {
        var userId = await CreateStudentAsync();
        var exam = await LoadSampleExamAsync();

        await using var scope = fixture.Services.CreateAsyncScope();
        var tests = scope.ServiceProvider.GetRequiredService<ITestService>();
        var attemptId = await tests.StartOrResumeAsync(userId, exam.ExamPaperId);
        foreach (var question in exam.Sections.SelectMany(s => s.Questions).Where(q => q.IsActive))
        {
            var saved = await tests.SaveAnswerAsync(userId, CorrectAnswer(attemptId, question));
            Assert.True(saved.Success, saved.Message);
        }

        Assert.Equal(attemptId, await tests.SubmitAsync(userId, attemptId));
        var result = await tests.GetResultAsync(attemptId, userId);
        Assert.Equal(attemptId, await tests.SubmitAsync(userId, attemptId));
        Assert.Equal(result?.StarsEarned, (await tests.GetResultAsync(attemptId, userId))?.StarsEarned);

        Assert.NotNull(result);
        Assert.Equal(100, result.PercentageScore);
        Assert.Equal(0, result.WrongCount);
        Assert.Equal(0, result.UnansweredCount);
        Assert.True(result.StarsEarned >= 3);
        Assert.All(result.Questions, q => Assert.True(q.IsCorrect));
        Assert.Null(await tests.GetResultAsync(attemptId, await CreateStudentAsync()));

        var history = await tests.GetHistoryAsync(userId);
        Assert.Contains(history, h => h.ExamAttemptId == attemptId && h.Status == nameof(AttemptStatus.Submitted));
        Assert.Empty(await tests.GetActiveAttemptsAsync(userId));
    }
}
