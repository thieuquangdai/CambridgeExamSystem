using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Web.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

[Authorize]
public class TranslationController(
    ITranslationService translationService,
    IVocabularyService vocabularyService,
    IValidator<TranslateRequest> translateValidator,
    IValidator<SaveVocabularyRequest> vocabularyValidator) : Controller
{
    public async Task<IActionResult> Index(string? search, bool bookmarked = false, CancellationToken cancellationToken = default)
    {
        ViewBag.Search = search;
        ViewBag.Bookmarked = bookmarked;
        return View(await vocabularyService.GetAsync(User.GetRequiredUserId(), search, bookmarked, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Translate([FromBody] TranslateRequest request, CancellationToken cancellationToken)
    {
        var validation = await translateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new TranslationResultDto { OriginalText = request.Text, ErrorMessage = validation.Errors[0].ErrorMessage });
        }

        return Json(await translationService.TranslateAsync(request, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveVocabularyRequest request, CancellationToken cancellationToken)
    {
        var validation = await vocabularyValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new { success = false, message = validation.Errors[0].ErrorMessage });
        }

        var saved = await vocabularyService.SaveAsync(User.GetRequiredUserId(), request, cancellationToken);
        return Json(new { success = true, vocabulary = saved });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBookmark(long id, CancellationToken cancellationToken)
    {
        var bookmarked = await vocabularyService.ToggleBookmarkAsync(User.GetRequiredUserId(), id, cancellationToken);
        return Json(new { success = true, isBookmarked = bookmarked });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        if (await vocabularyService.DeleteAsync(User.GetRequiredUserId(), id, cancellationToken))
        {
            TempData["Success"] = "Đã xoá từ vựng.";
        }

        return RedirectToAction(nameof(Index));
    }
}
