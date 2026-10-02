using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Domain.Enums;
using CambridgeExamSystem.Web.Infrastructure;
using CambridgeExamSystem.Web.Models.Leaderboard;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

public class LeaderboardController(ILeaderboardService leaderboardService, IExamService examService) : Controller
{
    public async Task<IActionResult> Index(LeaderboardPeriod period = LeaderboardPeriod.Weekly, int? levelId = null, CancellationToken cancellationToken = default)
    {
        return View(new LeaderboardViewModel
        {
            Board = await leaderboardService.GetLeaderboardAsync(period, levelId, User.GetUserId(), 50, cancellationToken),
            Levels = await examService.GetLevelsAsync(cancellationToken)
        });
    }
}
