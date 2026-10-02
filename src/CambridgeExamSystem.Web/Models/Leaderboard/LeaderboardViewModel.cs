using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Leaderboard;

public class LeaderboardViewModel
{
    public LeaderboardDto Board { get; set; } = new();
    public List<ExamLevelDto> Levels { get; set; } = [];
}
