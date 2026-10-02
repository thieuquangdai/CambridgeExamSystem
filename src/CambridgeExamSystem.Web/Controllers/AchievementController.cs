using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

[Authorize]
public class AchievementController(IAchievementService achievementService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await achievementService.GetUserAchievementsAsync(User.GetRequiredUserId(), cancellationToken));
}
