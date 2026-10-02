using CambridgeExamSystem.Application.Services;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Tests.Application;

public class RewardCalculatorTests
{
    [Theory]
    [InlineData(100, 3)]
    [InlineData(90, 3)]
    [InlineData(89.99, 2)]
    [InlineData(70, 2)]
    [InlineData(0, 1)]
    public void Exam_stars_depend_on_percentage(decimal percentage, int expected) =>
        Assert.Equal(expected, RewardCalculator.CalculateExamStars(percentage));

    [Theory]
    [InlineData(0, "Bronze")]
    [InlineData(30, "Silver")]
    [InlineData(100, "Gold")]
    [InlineData(250, "Platinum")]
    [InlineData(500, "Diamond")]
    public void Rank_depends_on_total_stars(int stars, string expected) =>
        Assert.Equal(expected, RewardCalculator.GetRankCode(stars));

    [Fact]
    public void Apply_attempt_updates_average_best_and_streak()
    {
        var day = new DateOnly(2026, 10, 1);
        var statistics = new UserStatistic { TotalTests = 1, PassedTests = 1, AverageScore = 80, BestPercentage = 80, CurrentStreakDays = 1, LongestStreakDays = 1, LastActivityDate = day };

        RewardCalculator.ApplyAttempt(statistics, 40, passed: false, secondsUsed: 120, day.AddDays(1), DateTime.UtcNow);

        Assert.Equal(2, statistics.TotalTests);
        Assert.Equal(1, statistics.PassedTests);
        Assert.Equal(60, statistics.AverageScore);
        Assert.Equal(80, statistics.BestPercentage);
        Assert.Equal(2, statistics.CurrentStreakDays);
        Assert.Equal(2, statistics.LongestStreakDays);
        Assert.Equal(120, statistics.TotalLearningSeconds);
    }

    [Fact]
    public void Streak_resets_after_a_missed_day_and_is_kept_on_same_day()
    {
        var day = new DateOnly(2026, 10, 1);
        var statistics = new UserStatistic { CurrentStreakDays = 5, LongestStreakDays = 5, LastActivityDate = day };

        RewardCalculator.ApplyAttempt(statistics, 50, false, 0, day, DateTime.UtcNow);
        Assert.Equal(5, statistics.CurrentStreakDays);

        RewardCalculator.ApplyAttempt(statistics, 50, false, 0, day.AddDays(3), DateTime.UtcNow);
        Assert.Equal(1, statistics.CurrentStreakDays);
        Assert.Equal(5, statistics.LongestStreakDays);
    }

    [Fact]
    public void Add_stars_updates_rank()
    {
        var statistics = new UserStatistic { TotalStars = 98 };

        RewardCalculator.AddStars(statistics, 3);

        Assert.Equal(101, statistics.TotalStars);
        Assert.Equal("Gold", statistics.CurrentRankCode);
    }
}
