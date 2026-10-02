using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Interfaces;

public interface IAchievementService
{
    Task<List<AchievementDto>> AwardEligibleAsync(UserStatistic statistics, TestAttempt attempt, CancellationToken cancellationToken = default);
    Task<UserAchievementsDto> GetUserAchievementsAsync(int userId, CancellationToken cancellationToken = default);
}
