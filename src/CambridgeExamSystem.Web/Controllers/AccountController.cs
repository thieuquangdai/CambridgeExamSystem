using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Web.Infrastructure;
using CambridgeExamSystem.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CambridgeExamSystem.Web.Controllers;

public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IExamService examService,
    ITestService testService,
    IAchievementService achievementService,
    TimeProvider timeProvider,
    ILogger<AccountController> logger) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản tạm thời bị khoá do đăng nhập sai nhiều lần. Vui lòng thử lại sau 15 phút.");
            return View(model);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        user.LastLoginAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        await userManager.UpdateAsync(user);
        logger.LogInformation("User {UserId} signed in", user.Id);
        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Register(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Levels = await examService.GetLevelsAsync(cancellationToken);
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.Email.Trim(),
                Email = model.Email.Trim(),
                FullName = model.FullName.Trim(),
                DateOfBirth = model.DateOfBirth,
                CurrentLevelCode = string.IsNullOrWhiteSpace(model.CurrentLevelCode) ? null : model.CurrentLevelCode,
                CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, RoleNames.Student);
                await signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Đăng ký thành công. Chào mừng bạn!";
                return RedirectToAction("Index", "Exam");
            }

            ModelState.AddErrors(result.Errors.Select(e => e.Description));
        }

        ViewBag.Levels = await examService.GetLevelsAsync(cancellationToken);
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var achievements = await achievementService.GetUserAchievementsAsync(user.Id, cancellationToken);
        var history = await testService.GetHistoryAsync(user.Id, cancellationToken);
        return View(new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            CurrentLevelCode = user.CurrentLevelCode,
            Roles = await userManager.GetRolesAsync(user),
            Statistics = achievements.Statistics,
            EarnedAchievements = achievements.EarnedCount,
            ActiveAttempts = await testService.GetActiveAttemptsAsync(user.Id, cancellationToken),
            RecentAttempts = history.Take(5).ToList()
        });
    }

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
}
