using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Infrastructure.Data;
using CambridgeExamSystem.Infrastructure.Repositories;
using CambridgeExamSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CambridgeExamSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<CambridgeDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(CambridgeDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure(3);
            }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<IVocabularyRepository, VocabularyRepository>();

        services.AddMemoryCache();
        services.Configure<TranslationOptions>(configuration.GetSection(TranslationOptions.SectionName));
        services.AddHttpClient<ITranslationService, TranslationService>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<TranslationOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("CambridgeExamSystem/1.0");
        });

        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<DbInitializer>();
        return services;
    }
}
