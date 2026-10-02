using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CambridgeExamSystem.Infrastructure.Data;

public sealed class DbInitializer(
    CambridgeDbContext context,
    RoleManager<Role> roleManager,
    UserManager<User> userManager,
    IOptions<SeedOptions> options,
    TimeProvider timeProvider,
    ILogger<DbInitializer> logger)
{
    private readonly SeedOptions _options = options.Value;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_options.ApplyMigrations && context.Database.IsRelational())
        {
            await context.Database.MigrateAsync(cancellationToken);
        }

        await SeedRolesAsync();
        await SeedReferenceDataAsync(cancellationToken);
        var adminId = await SeedAdminAsync();

        if (_options.SampleData)
        {
            await SampleExamSeeder.SeedAsync(context, adminId, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role(roleName));
            }
        }
    }

    private async Task<int?> SeedAdminAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.AdminEmail))
        {
            return null;
        }

        var admin = await userManager.FindByEmailAsync(_options.AdminEmail);
        if (admin is null)
        {
            if (string.IsNullOrWhiteSpace(_options.AdminPassword))
            {
                logger.LogWarning("Seed:AdminEmail is set but Seed:AdminPassword is empty; the admin account was not created. Set it via user-secrets or environment variable Seed__AdminPassword.");
                return null;
            }

            admin = new User
            {
                UserName = _options.AdminEmail,
                Email = _options.AdminEmail,
                EmailConfirmed = true,
                FullName = _options.AdminFullName,
                CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
            };

            var created = await userManager.CreateAsync(admin, _options.AdminPassword);
            if (!created.Succeeded)
            {
                logger.LogError("Could not create admin account: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
                return null;
            }

            logger.LogInformation("Created admin account {Email}", _options.AdminEmail);
        }

        if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
        {
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);
        }

        return admin.Id;
    }

    private async Task SeedReferenceDataAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var levelCodes = await context.ExamLevels.Select(l => l.LevelCode).ToListAsync(cancellationToken);
        context.ExamLevels.AddRange(ReferenceData.Levels().Where(l => !levelCodes.Contains(l.LevelCode)));

        var typeCodes = await context.QuestionTypes.Select(t => t.TypeCode).ToListAsync(cancellationToken);
        context.QuestionTypes.AddRange(ReferenceData.QuestionTypes().Where(t => !typeCodes.Contains(t.TypeCode)));

        var achievementCodes = await context.Achievements.Select(a => a.AchievementCode).ToListAsync(cancellationToken);
        context.Achievements.AddRange(ReferenceData.Achievements().Where(a => !achievementCodes.Contains(a.AchievementCode)));

        if (!await context.MotivationalMessages.AnyAsync(cancellationToken))
        {
            context.MotivationalMessages.AddRange(ReferenceData.MotivationalMessages(now));
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
