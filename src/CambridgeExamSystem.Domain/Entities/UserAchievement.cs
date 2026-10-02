namespace CambridgeExamSystem.Domain.Entities;

public class UserAchievement
{
    public long UserAchievementId { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime EarnedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
