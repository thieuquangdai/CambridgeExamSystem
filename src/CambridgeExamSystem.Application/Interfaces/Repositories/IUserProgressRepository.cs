using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Interfaces.Repositories;

public interface IUserProgressRepository
{
    Task<UserStatistic?> GetStatisticsAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Achievement>> GetActiveAchievementsAsync(CancellationToken cancellationToken = default);
    Task<List<UserAchievement>> GetUserAchievementsAsync(int userId, CancellationToken cancellationToken = default);
    Task<MotivationalMessage?> GetMotivationalMessageAsync(decimal percentage, CancellationToken cancellationToken = default);
    Task<List<UserAchievement>> GetAchievementsEarnedForAttemptAsync(int userId, DateTime earnedFromUtc, DateTime earnedToUtc, CancellationToken cancellationToken = default);
    void AddStatistics(UserStatistic statistics);
    void AddUserAchievement(UserAchievement userAchievement);
    void AddStarTransaction(StarTransaction starTransaction);
}
