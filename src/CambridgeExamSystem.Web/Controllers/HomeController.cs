using System.Diagnostics;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Web.Infrastructure;
using CambridgeExamSystem.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

public class HomeController(IExamService examService, ITestService testService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var model = new HomeViewModel
        {
            Levels = await examService.GetLevelsAsync(cancellationToken),
            LatestExams = (await examService.GetPublishedExamsAsync(null, userId, cancellationToken)).Take(6).ToList()
        };

        if (userId is int id)
        {
            model.ActiveAttempts = await testService.GetActiveAttemptsAsync(id, cancellationToken);
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    [Route("Home/StatusCode")]
    public IActionResult StatusCodePage(int code)
    {
        Response.StatusCode = code;
        return View("Error", new ErrorViewModel { StatusCode = code, RequestId = HttpContext.TraceIdentifier });
    }
}
