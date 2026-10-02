using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Account;

public class ProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CurrentLevelCode { get; set; }
    public IList<string> Roles { get; set; } = [];
    public UserStatisticsDto Statistics { get; set; } = new();
    public int EarnedAchievements { get; set; }
    public List<ActiveAttemptDto> ActiveAttempts { get; set; } = [];
    public List<AttemptHistoryItemDto> RecentAttempts { get; set; } = [];
}
