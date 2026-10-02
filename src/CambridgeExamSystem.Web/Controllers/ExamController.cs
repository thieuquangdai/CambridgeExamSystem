using CambridgeExamSystem.Application.Common;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Web.Infrastructure;
using CambridgeExamSystem.Web.Models.Exam;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

[Authorize]
public class ExamController(
    IExamService examService,
    ITestService testService,
    IValidator<SaveAnswerRequest> saveAnswerValidator) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? level, CancellationToken cancellationToken)
    {
        return View(new ExamIndexViewModel
        {
            Levels = await examService.GetLevelsAsync(cancellationToken),
            Exams = await examService.GetPublishedExamsAsync(level, User.GetUserId(), cancellationToken),
            SelectedLevel = level
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var exam = await examService.GetExamDetailAsync(id, User.GetUserId(), cancellationToken);
        return exam is null ? NotFound() : View(exam);
    }

    [HttpPost]
    public async Task<IActionResult> Start(int id, CancellationToken cancellationToken)
    {
        try
        {
            var attemptId = await testService.StartOrResumeAsync(User.GetRequiredUserId(), id, cancellationToken);
            return RedirectToAction(nameof(Take), new { id = attemptId });
        }
        catch (Exception ex) when (ex is NotFoundException or BusinessRuleException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Take(long id, CancellationToken cancellationToken)
    {
        var test = await testService.GetTakeTestAsync(id, User.GetRequiredUserId(), cancellationToken);
        if (test is null)
        {
            TempData["Info"] = "Bài làm đã kết thúc. Đây là kết quả của bạn.";
            return RedirectToAction("Details", "Result", new { id });
        }

        return View(test);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAnswer([FromBody] SaveAnswerRequest request, CancellationToken cancellationToken)
    {
        var validation = await saveAnswerValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new SaveAnswerResult { Success = false, Message = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)) });
        }

        var result = await testService.SaveAnswerAsync(User.GetRequiredUserId(), request, cancellationToken);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> Pause([FromForm] PauseRequest request, CancellationToken cancellationToken)
    {
        await testService.PauseAsync(User.GetRequiredUserId(), request.ExamAttemptId, request.CurrentQuestionIndex, Math.Max(0, request.TimeSpentSeconds), cancellationToken);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Submit(long examAttemptId, CancellationToken cancellationToken)
    {
        try
        {
            var attemptId = await testService.SubmitAsync(User.GetRequiredUserId(), examAttemptId, cancellationToken);
            return RedirectToAction("Details", "Result", new { id = attemptId });
        }
        catch (Exception ex) when (ex is NotFoundException or BusinessRuleException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("History", "Result");
        }
    }
}
