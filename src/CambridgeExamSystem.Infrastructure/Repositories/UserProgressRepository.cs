using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Repositories;

public sealed class UserProgressRepository(CambridgeDbContext context) : IUserProgressRepository
{
    public Task<UserStatistic?> GetStatisticsAsync(int userId, CancellationToken cancellationToken = default) =>
        context.UserStatistics.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

    public Task<List<Achievement>> GetActiveAchievementsAsync(CancellationToken cancellationToken = default) =>
        context.Achievements
            .Where(a => a.IsActive)
            .OrderBy(a => a.RequiredTests ?? 0)
            .ThenBy(a => a.RequiredScore ?? 0)
            .ToListAsync(cancellationToken);

    public Task<List<UserAchievement>> GetUserAchievementsAsync(int userId, CancellationToken cancellationToken = default) =>
        context.UserAchievements.Where(ua => ua.UserId == userId).ToListAsync(cancellationToken);

    public async Task<MotivationalMessage?> GetMotivationalMessageAsync(decimal percentage, CancellationToken cancellationToken = default)
    {
        var candidates = await context.MotivationalMessages
            .AsNoTracking()
            .Where(m => m.IsActive
                && (m.MinPercentage == null || m.MinPercentage <= percentage)
                && (m.MaxPercentage == null || m.MaxPercentage >= percentage))
            .ToListAsync(cancellationToken);

        return candidates.Count == 0 ? null : candidates[Random.Shared.Next(candidates.Count)];
    }

    public Task<List<UserAchievement>> GetAchievementsEarnedForAttemptAsync(int userId, DateTime earnedFromUtc, DateTime earnedToUtc, CancellationToken cancellationToken = default) =>
        context.UserAchievements
            .AsNoTracking()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId && ua.EarnedAtUtc >= earnedFromUtc && ua.EarnedAtUtc <= earnedToUtc)
            .ToListAsync(cancellationToken);

    public void AddStatistics(UserStatistic statistics) => context.UserStatistics.Add(statistics);

    public void AddUserAchievement(UserAchievement userAchievement) => context.UserAchievements.Add(userAchievement);

    public void AddStarTransaction(StarTransaction starTransaction) => context.StarTransactions.Add(starTransaction);
}
