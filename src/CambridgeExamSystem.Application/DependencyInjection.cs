using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Mappings;
using CambridgeExamSystem.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CambridgeExamSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, string? autoMapperLicenseKey = null)
    {
        services.AddAutoMapper(cfg =>
        {
            if (!string.IsNullOrWhiteSpace(autoMapperLicenseKey))
            {
                cfg.LicenseKey = autoMapperLicenseKey;
            }
        }, typeof(MappingProfile).Assembly);

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IGradingService, GradingService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IVocabularyService, VocabularyService>();
        return services;
    }
}
