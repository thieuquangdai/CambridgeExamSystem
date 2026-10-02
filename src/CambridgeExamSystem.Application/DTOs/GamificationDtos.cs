using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Application.DTOs;

public sealed class AchievementDto
{
    public int AchievementId { get; set; }
    public string AchievementCode { get; set; } = string.Empty;
    public string AchievementName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? BadgeColor { get; set; }
    public int? RequiredTests { get; set; }
    public decimal? RequiredScore { get; set; }
    public int RewardStars { get; set; }
    public bool IsEarned { get; set; }
    public DateTime? EarnedAtUtc { get; set; }
}

public sealed class UserStatisticsDto
{
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int TotalStars { get; set; }
    public decimal AverageScore { get; set; }
    public decimal BestPercentage { get; set; }
    public string CurrentRankCode { get; set; } = "Bronze";
    public int CurrentStreakDays { get; set; }
    public int LongestStreakDays { get; set; }
    public long TotalLearningSeconds { get; set; }
}

public sealed class UserAchievementsDto
{
    public UserStatisticsDto Statistics { get; set; } = new();
    public List<AchievementDto> Achievements { get; set; } = [];
    public int EarnedCount => Achievements.Count(a => a.IsEarned);
}

public sealed class LeaderboardEntryDto
{
    public int RankNumber { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int Stars { get; set; }
    public decimal AverageScore { get; set; }
    public int TotalTests { get; set; }
    public string RankCode { get; set; } = "Bronze";
    public bool IsCurrentUser { get; set; }
}

public sealed class LeaderboardDto
{
    public LeaderboardPeriod Period { get; set; }
    public int? LevelId { get; set; }
    public DateOnly? PeriodStartDate { get; set; }
    public DateOnly? PeriodEndDate { get; set; }
    public List<LeaderboardEntryDto> Entries { get; set; } = [];
    public LeaderboardEntryDto? CurrentUserEntry { get; set; }
}
