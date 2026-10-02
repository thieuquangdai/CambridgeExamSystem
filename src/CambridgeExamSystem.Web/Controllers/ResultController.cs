using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Web.Infrastructure;
using CambridgeExamSystem.Web.Models.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

[Authorize]
public class ResultController(ITestService testService) : Controller
{
    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken)
    {
        var result = await testService.GetResultAsync(id, User.GetRequiredUserId(), cancellationToken);
        if (result is null)
        {
            TempData["Error"] = "Không tìm thấy kết quả của bài làm này.";
            return RedirectToAction(nameof(History));
        }

        return View(result);
    }

    public async Task<IActionResult> History(CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        return View(new HistoryViewModel
        {
            Attempts = (await testService.GetHistoryAsync(userId, cancellationToken)).Where(a => a.SubmittedAtUtc is not null).ToList(),
            ActiveAttempts = await testService.GetActiveAttemptsAsync(userId, cancellationToken)
        });
    }
}
