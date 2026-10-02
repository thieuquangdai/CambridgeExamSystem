using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Repositories;

public sealed class LeaderboardRepository(CambridgeDbContext context) : ILeaderboardRepository
{
    public async Task<List<LeaderboardEntryDto>> GetEntriesAsync(DateTime? fromUtc, DateTime? toUtc, int? levelId, CancellationToken cancellationToken = default)
    {
        var attempts = context.TestAttempts.Where(a => a.IsSubmitted && a.PercentageScore != null && a.User.IsActive);
        var stars = context.StarTransactions.Where(t => t.User.IsActive);

        if (fromUtc is DateTime from)
        {
            attempts = attempts.Where(a => a.SubmittedAtUtc >= from);
            stars = stars.Where(t => t.CreatedAtUtc >= from);
        }

        if (toUtc is DateTime to)
        {
            attempts = attempts.Where(a => a.SubmittedAtUtc < to);
            stars = stars.Where(t => t.CreatedAtUtc < to);
        }

        if (levelId is int level)
        {
            attempts = attempts.Where(a => a.ExamPaper.LevelId == level);
            stars = stars.Where(t => t.TestAttempt != null && t.TestAttempt.ExamPaper.LevelId == level);
        }

        var attemptStats = await attempts
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, TotalTests = g.Count(), AverageScore = g.Average(a => a.PercentageScore!.Value) })
            .ToDictionaryAsync(x => x.UserId, cancellationToken);

        var starTotals = await stars
            .GroupBy(t => t.UserId)
            .Select(g => new { UserId = g.Key, Stars = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(x => x.UserId, x => x.Stars, cancellationToken);

        var userIds = attemptStats.Keys.Union(starTotals.Keys).ToList();
        var users = await context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName, u.UserName, u.AvatarUrl })
            .ToListAsync(cancellationToken);

        return users.Select(u =>
        {
            attemptStats.TryGetValue(u.Id, out var stat);
            return new LeaderboardEntryDto
            {
                UserId = u.Id,
                DisplayName = string.IsNullOrWhiteSpace(u.FullName) ? u.UserName ?? $"#{u.Id}" : u.FullName,
                AvatarUrl = u.AvatarUrl,
                Stars = starTotals.GetValueOrDefault(u.Id),
                TotalTests = stat?.TotalTests ?? 0,
                AverageScore = Math.Round(stat?.AverageScore ?? 0, 2)
            };
        }).ToList();
    }

    public void AddSnapshots(IEnumerable<LeaderboardSnapshot> snapshots) => context.LeaderboardSnapshots.AddRange(snapshots);
}
