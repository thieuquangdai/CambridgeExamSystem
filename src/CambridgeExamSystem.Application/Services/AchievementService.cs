using AutoMapper;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Application.Services;

public sealed class AchievementService(
    IUserProgressRepository progressRepository,
    IMapper mapper) : IAchievementService
{
    public async Task<List<AchievementDto>> AwardEligibleAsync(UserStatistic statistics, TestAttempt attempt, CancellationToken cancellationToken = default)
    {
        var achievements = await progressRepository.GetActiveAchievementsAsync(cancellationToken);
        var earnedIds = (await progressRepository.GetUserAchievementsAsync(statistics.UserId, cancellationToken))
            .Select(ua => ua.AchievementId)
            .ToHashSet();
        var percentage = attempt.PercentageScore ?? 0;
        var earnedAt = attempt.SubmittedAtUtc ?? statistics.UpdatedAtUtc;
        var awarded = new List<AchievementDto>();

        foreach (var achievement in achievements.Where(a => !earnedIds.Contains(a.AchievementId)))
        {
            if (!IsEligible(achievement, statistics.TotalTests, percentage))
            {
                continue;
            }

            progressRepository.AddUserAchievement(new UserAchievement
            {
                UserId = statistics.UserId,
                AchievementId = achievement.AchievementId,
                EarnedAtUtc = earnedAt
            });

            if (achievement.RewardStars > 0)
            {
                RewardCalculator.AddStars(statistics, achievement.RewardStars);
                progressRepository.AddStarTransaction(new StarTransaction
                {
                    UserId = statistics.UserId,
                    TestAttempt = attempt,
                    Amount = achievement.RewardStars,
                    TransactionType = StarTransactionType.ACHIEVEMENT_REWARD,
                    Description = $"Thành tích: {achievement.AchievementName}",
                    CreatedAtUtc = earnedAt
                });
            }

            var dto = mapper.Map<AchievementDto>(achievement);
            dto.IsEarned = true;
            dto.EarnedAtUtc = earnedAt;
            awarded.Add(dto);
        }

        return awarded;
    }

    public async Task<UserAchievementsDto> GetUserAchievementsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var achievements = await progressRepository.GetActiveAchievementsAsync(cancellationToken);
        var earned = (await progressRepository.GetUserAchievementsAsync(userId, cancellationToken))
            .ToDictionary(ua => ua.AchievementId, ua => ua.EarnedAtUtc);
        var statistics = await progressRepository.GetStatisticsAsync(userId, cancellationToken);

        return new UserAchievementsDto
        {
            Statistics = statistics is null ? new UserStatisticsDto() : mapper.Map<UserStatisticsDto>(statistics),
            Achievements = achievements.Select(a =>
            {
                var dto = mapper.Map<AchievementDto>(a);
                dto.IsEarned = earned.TryGetValue(a.AchievementId, out var earnedAt);
                dto.EarnedAtUtc = dto.IsEarned ? earnedAt : null;
                return dto;
            }).ToList()
        };
    }

    public static bool IsEligible(Achievement achievement, int totalTests, decimal percentage)
    {
        if (achievement.RequiredTests is null && achievement.RequiredScore is null)
        {
            return false;
        }

        return (achievement.RequiredTests is null || totalTests >= achievement.RequiredTests)
            && (achievement.RequiredScore is null || percentage >= achievement.RequiredScore);
    }
}
