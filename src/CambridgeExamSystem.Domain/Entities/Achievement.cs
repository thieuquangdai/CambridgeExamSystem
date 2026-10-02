namespace CambridgeExamSystem.Domain.Entities;

public class Achievement
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
    public bool IsActive { get; set; } = true;

    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
