using Microsoft.AspNetCore.Identity;

namespace CambridgeExamSystem.Domain.Entities;

public class User : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CurrentLevelCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }

    public UserStatistic? Statistics { get; set; }
    public ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
    public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    public ICollection<UserVocabulary> Vocabulary { get; set; } = new List<UserVocabulary>();
    public ICollection<StarTransaction> StarTransactions { get; set; } = new List<StarTransaction>();
}
