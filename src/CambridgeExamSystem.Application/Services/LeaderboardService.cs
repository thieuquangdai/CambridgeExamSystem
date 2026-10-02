using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Application.Services;

public sealed class LeaderboardService(
    ILeaderboardRepository leaderboardRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ILeaderboardService
{
    public async Task<LeaderboardDto> GetLeaderboardAsync(LeaderboardPeriod period, int? levelId, int? currentUserId, int top = 50, CancellationToken cancellationToken = default)
    {
        var (start, end) = GetPeriodRange(period, DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
        var ranked = await GetRankedEntriesAsync(start, end, levelId, cancellationToken);

        foreach (var entry in ranked)
        {
            entry.IsCurrentUser = entry.UserId == currentUserId;
        }

        return new LeaderboardDto
        {
            Period = period,
            LevelId = levelId,
            PeriodStartDate = start,
            PeriodEndDate = end,
            Entries = ranked.Take(top).ToList(),
            CurrentUserEntry = ranked.FirstOrDefault(e => e.IsCurrentUser)
        };
    }

    public async Task<int> CreateSnapshotAsync(LeaderboardPeriod period, int? levelId, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(now);
        var (start, end) = GetPeriodRange(period, today);
        var ranked = await GetRankedEntriesAsync(start, end, levelId, cancellationToken);

        leaderboardRepository.AddSnapshots(ranked.Select(e => new LeaderboardSnapshot
        {
            UserId = e.UserId,
            LevelId = levelId,
            PeriodType = period,
            PeriodStartDate = start ?? DateOnly.MinValue,
            PeriodEndDate = end ?? today,
            RankNumber = e.RankNumber,
            Stars = e.Stars,
            AverageScore = e.AverageScore,
            TotalTests = e.TotalTests,
            CreatedAtUtc = now
        }));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ranked.Count;
    }

    public static (DateOnly? Start, DateOnly? End) GetPeriodRange(LeaderboardPeriod period, DateOnly today)
    {
        switch (period)
        {
            case LeaderboardPeriod.Weekly:
                var offset = ((int)today.DayOfWeek + 6) % 7;
                var monday = today.AddDays(-offset);
                return (monday, monday.AddDays(6));
            case LeaderboardPeriod.Monthly:
                var first = new DateOnly(today.Year, today.Month, 1);
                return (first, first.AddMonths(1).AddDays(-1));
            default:
                return (null, null);
        }
    }

    private async Task<List<LeaderboardEntryDto>> GetRankedEntriesAsync(DateOnly? start, DateOnly? end, int? levelId, CancellationToken cancellationToken)
    {
        var fromUtc = start?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc = end?.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var entries = await leaderboardRepository.GetEntriesAsync(fromUtc, toUtc, levelId, cancellationToken);

        var ranked = entries
            .OrderByDescending(e => e.Stars)
            .ThenByDescending(e => e.AverageScore)
            .ThenByDescending(e => e.TotalTests)
            .ThenBy(e => e.DisplayName)
            .ToList();

        for (var i = 0; i < ranked.Count; i++)
        {
            ranked[i].RankNumber = i + 1;
            ranked[i].RankCode = RewardCalculator.GetRankCode(ranked[i].Stars);
        }

        return ranked;
    }
}
