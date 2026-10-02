using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Services;

public static class RewardCalculator
{
    public static int CalculateExamStars(decimal percentage) => percentage switch
    {
        >= 90 => 3,
        >= 70 => 2,
        _ => 1
    };

    public static string GetRankCode(int totalStars) => totalStars switch
    {
        >= 500 => "Diamond",
        >= 250 => "Platinum",
        >= 100 => "Gold",
        >= 30 => "Silver",
        _ => "Bronze"
    };

    public static void ApplyAttempt(UserStatistic statistics, decimal percentage, bool passed, int secondsUsed, DateOnly activityDate, DateTime nowUtc)
    {
        var previousTotal = statistics.TotalTests;
        statistics.TotalTests = previousTotal + 1;
        statistics.PassedTests += passed ? 1 : 0;
        statistics.AverageScore = Math.Round(((statistics.AverageScore * previousTotal) + percentage) / statistics.TotalTests, 2);
        statistics.BestPercentage = Math.Max(statistics.BestPercentage, percentage);
        statistics.TotalLearningSeconds += Math.Max(0, secondsUsed);

        if (statistics.LastActivityDate == activityDate)
        {
            statistics.CurrentStreakDays = Math.Max(1, statistics.CurrentStreakDays);
        }
        else if (statistics.LastActivityDate == activityDate.AddDays(-1))
        {
            statistics.CurrentStreakDays += 1;
        }
        else
        {
            statistics.CurrentStreakDays = 1;
        }

        statistics.LongestStreakDays = Math.Max(statistics.LongestStreakDays, statistics.CurrentStreakDays);
        statistics.LastActivityDate = activityDate;
        statistics.UpdatedAtUtc = nowUtc;
    }

    public static void AddStars(UserStatistic statistics, int stars)
    {
        statistics.TotalStars += stars;
        statistics.CurrentRankCode = GetRankCode(statistics.TotalStars);
    }
}
