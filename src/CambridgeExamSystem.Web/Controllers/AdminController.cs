using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Domain.Enums;
using CambridgeExamSystem.Web.Infrastructure;
using CambridgeExamSystem.Web.Models.Admin;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Web.Controllers;

[Authorize(Roles = RoleNames.AdminOrTeacher)]
public class AdminController(
    IExamService examService,
    ILeaderboardService leaderboardService,
    IFileStorageService fileStorage,
    UserManager<User> userManager,
    IValidator<ExamPaperEditDto> examValidator,
    IValidator<SectionEditDto> sectionValidator,
    IValidator<QuestionEditDto> questionValidator) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await examService.GetDashboardAsync(cancellationToken));

    public async Task<IActionResult> Exams(CancellationToken cancellationToken) =>
        View(await examService.GetAllExamsAsync(cancellationToken));

    [HttpGet]
    public async Task<IActionResult> CreateExam(CancellationToken cancellationToken) =>
        View("ExamForm", new ExamFormViewModel { Levels = await examService.GetLevelsAsync(cancellationToken) });

    [HttpGet]
    public async Task<IActionResult> EditExam(int id, CancellationToken cancellationToken)
    {
        var exam = await examService.GetExamForEditAsync(id, cancellationToken);
        if (exam is null)
        {
            return NotFound();
        }

        return View("ExamForm", new ExamFormViewModel { Exam = exam, Levels = await examService.GetLevelsAsync(cancellationToken) });
    }

    [HttpPost]
    public async Task<IActionResult> SaveExam(ExamFormViewModel model, CancellationToken cancellationToken)
    {
        if (await examValidator.ValidateIntoAsync(model.Exam, ModelState, nameof(model.Exam)) && ModelState.IsValid)
        {
            var result = await examService.SaveExamAsync(model.Exam, User.GetUserId(), cancellationToken);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đã lưu đề thi.";
                return RedirectToAction(nameof(Content), new { id = result.Value });
            }

            ModelState.AddErrors(result.Errors);
        }

        model.Levels = await examService.GetLevelsAsync(cancellationToken);
        return View("ExamForm", model);
    }

    [HttpPost]
    public async Task<IActionResult> TogglePublish(int id, bool publish, CancellationToken cancellationToken)
    {
        var result = await examService.SetPublishedAsync(id, publish, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? (publish ? "Đã xuất bản đề thi." : "Đã ẩn đề thi.")
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Content), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Content(int id, CancellationToken cancellationToken)
    {
        var content = await examService.GetExamContentAsync(id, cancellationToken);
        if (content is null)
        {
            return NotFound();
        }

        return View(new ExamContentViewModel { Content = content, NewSection = new SectionEditDto { ExamPaperId = id } });
    }

    [HttpPost]
    public async Task<IActionResult> AddSection(ExamContentViewModel model, CancellationToken cancellationToken)
    {
        var section = model.NewSection;
        var validation = await sectionValidator.ValidateAsync(section, cancellationToken);
        if (!validation.IsValid)
        {
            TempData["Error"] = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
        }
        else
        {
            var result = await examService.AddSectionAsync(section, cancellationToken);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Đã thêm phần thi." : string.Join(" ", result.Errors);
        }

        return RedirectToAction(nameof(Content), new { id = section.ExamPaperId });
    }

    [HttpGet]
    public async Task<IActionResult> AddQuestion(int examPaperId, int sectionId, CancellationToken cancellationToken)
    {
        var model = await BuildQuestionFormAsync(examPaperId, sectionId, null, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [RequestSizeLimit(60 * 1024 * 1024)]
    public async Task<IActionResult> AddQuestion(QuestionFormViewModel model, CancellationToken cancellationToken)
    {
        if (await questionValidator.ValidateIntoAsync(model.Question, ModelState, nameof(model.Question)) && ModelState.IsValid)
        {
            try
            {
                if (model.ImageFile is { Length: > 0 })
                {
                    await using var stream = model.ImageFile.OpenReadStream();
                    model.Question.ImageUrl = await fileStorage.SaveAsync(stream, model.ImageFile.FileName, "images", cancellationToken);
                }

                if (model.AudioFile is { Length: > 0 })
                {
                    await using var stream = model.AudioFile.OpenReadStream();
                    model.Question.AudioUrl = await fileStorage.SaveAsync(stream, model.AudioFile.FileName, "audio", cancellationToken);
                }

                var result = await examService.AddQuestionAsync(model.Question, cancellationToken);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Đã thêm câu hỏi.";
                    return RedirectToAction(nameof(Content), new { id = model.ExamPaperId });
                }

                ModelState.AddErrors(result.Errors);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        var rebuilt = await BuildQuestionFormAsync(model.ExamPaperId, model.Question.ExamSectionId, model.Question, cancellationToken);
        return rebuilt is null ? NotFound() : View(rebuilt);
    }

    [HttpPost]
    public async Task<IActionResult> DeactivateQuestion(int id, int examPaperId, CancellationToken cancellationToken)
    {
        var result = await examService.DeactivateQuestionAsync(id, cancellationToken);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Đã ẩn câu hỏi." : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Content), new { id = result.Succeeded ? result.Value : examPaperId });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> CreateSnapshot(LeaderboardPeriod period, CancellationToken cancellationToken)
    {
        var count = await leaderboardService.CreateSnapshotAsync(period, null, cancellationToken);
        TempData["Success"] = $"Đã lưu snapshot bảng xếp hạng ({count} học viên).";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.OrderByDescending(u => u.CreatedAtUtc).Take(200).ToListAsync(cancellationToken);
        var rows = new List<UserRowViewModel>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            rows.Add(new UserRowViewModel
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAtUtc = user.CreatedAtUtc,
                LastLoginAtUtc = user.LastLoginAtUtc,
                Role = roles.FirstOrDefault() ?? string.Empty
            });
        }

        return View(rows);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> SetRole(int userId, string role)
    {
        if (!RoleNames.All.Contains(role))
        {
            TempData["Error"] = "Vai trò không hợp lệ.";
            return RedirectToAction(nameof(Users));
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        if (user.Id == User.GetUserId() && role != RoleNames.Admin)
        {
            TempData["Error"] = "Bạn không thể tự gỡ quyền Admin của chính mình.";
            return RedirectToAction(nameof(Users));
        }

        var current = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, current);
        await userManager.AddToRoleAsync(user, role);
        TempData["Success"] = $"Đã cập nhật vai trò của {user.FullName} thành {role}.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> ToggleActive(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound();
        }

        if (user.Id == User.GetUserId())
        {
            TempData["Error"] = "Bạn không thể khoá tài khoản của chính mình.";
            return RedirectToAction(nameof(Users));
        }

        user.IsActive = !user.IsActive;
        await userManager.UpdateAsync(user);
        await userManager.UpdateSecurityStampAsync(user);
        TempData["Success"] = user.IsActive ? "Đã mở khoá tài khoản." : "Đã khoá tài khoản.";
        return RedirectToAction(nameof(Users));
    }

    private async Task<QuestionFormViewModel?> BuildQuestionFormAsync(int examPaperId, int sectionId, QuestionEditDto? question, CancellationToken cancellationToken)
    {
        var content = await examService.GetExamContentAsync(examPaperId, cancellationToken);
        var section = content?.Sections.FirstOrDefault(s => s.ExamSectionId == sectionId);
        if (content is null || section is null)
        {
            return null;
        }

        question ??= new QuestionEditDto { ExamSectionId = sectionId };
        while (question.Options.Count < 4)
        {
            question.Options.Add(new OptionEditDto());
        }

        return new QuestionFormViewModel
        {
            ExamPaperId = examPaperId,
            ExamName = content.Exam.ExamName,
            SectionName = section.SectionName,
            Question = question,
            QuestionTypes = await examService.GetQuestionTypesAsync(cancellationToken)
        };
    }
}
