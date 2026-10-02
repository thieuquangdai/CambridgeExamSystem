using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Repositories;

public sealed class TestAttemptRepository(CambridgeDbContext context) : ITestAttemptRepository
{
    private IQueryable<TestAttempt> OpenAttempts(int userId) =>
        context.TestAttempts.Where(a => a.UserId == userId
            && !a.IsSubmitted
            && (a.Status == AttemptStatus.InProgress || a.Status == AttemptStatus.Paused));

    public Task<TestAttempt?> GetOpenAttemptAsync(int userId, int examPaperId, CancellationToken cancellationToken = default) =>
        OpenAttempts(userId)
            .Where(a => a.ExamPaperId == examPaperId)
            .OrderByDescending(a => a.LastSavedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<TestAttempt>> GetOpenAttemptsAsync(int userId, CancellationToken cancellationToken = default) =>
        OpenAttempts(userId)
            .AsNoTracking()
            .AsSplitQuery()
            .Include(a => a.ExamPaper).ThenInclude(p => p.Level)
            .Include(a => a.Answers)
            .OrderByDescending(a => a.LastSavedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<TestAttempt?> GetWithAnswersAsync(long examAttemptId, int userId, CancellationToken cancellationToken = default) =>
        context.TestAttempts
            .AsSplitQuery()
            .Include(a => a.Answers)
            .Include(a => a.Result)
            .FirstOrDefaultAsync(a => a.ExamAttemptId == examAttemptId && a.UserId == userId, cancellationToken);

    public Task<int> CountAttemptsAsync(int userId, int examPaperId, CancellationToken cancellationToken = default) =>
        context.TestAttempts.CountAsync(a => a.UserId == userId && a.ExamPaperId == examPaperId, cancellationToken);

    public Task<List<TestAttempt>> GetUserAttemptsAsync(int userId, int? examPaperId, CancellationToken cancellationToken = default)
    {
        var query = context.TestAttempts
            .AsNoTracking()
            .Include(a => a.ExamPaper).ThenInclude(p => p.Level)
            .Include(a => a.Result)
            .Where(a => a.UserId == userId);

        if (examPaperId is int id)
        {
            query = query.Where(a => a.ExamPaperId == id);
        }

        return query.OrderByDescending(a => a.StartedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<(int Total, int Submitted, decimal AveragePercentage)> GetAttemptTotalsAsync(CancellationToken cancellationToken = default)
    {
        var total = await context.TestAttempts.CountAsync(cancellationToken);
        var submitted = context.TestAttempts.Where(a => a.IsSubmitted && a.PercentageScore != null);
        var submittedCount = await submitted.CountAsync(cancellationToken);
        var average = submittedCount == 0 ? 0 : await submitted.AverageAsync(a => a.PercentageScore!.Value, cancellationToken);
        return (total, submittedCount, Math.Round(average, 2));
    }

    public void Add(TestAttempt attempt) => context.TestAttempts.Add(attempt);

    public void AddEvent(AttemptEvent attemptEvent) => context.AttemptEvents.Add(attemptEvent);
}
