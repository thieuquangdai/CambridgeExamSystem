namespace CambridgeExamSystem.Domain.Entities;

public class UserStatistic
{
    public int UserId { get; set; }
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int TotalStars { get; set; }
    public decimal AverageScore { get; set; }
    public decimal BestPercentage { get; set; }
    public string CurrentRankCode { get; set; } = "Bronze";
    public int CurrentStreakDays { get; set; }
    public int LongestStreakDays { get; set; }
    public DateOnly? LastActivityDate { get; set; }
    public long TotalLearningSeconds { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
