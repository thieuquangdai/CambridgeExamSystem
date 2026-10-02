using CambridgeExamSystem.Application;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Infrastructure;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CambridgeExamSystem.Tests.Infrastructure;

public sealed class SqlServerDatabaseFixture : IAsyncLifetime
{
    private ServiceProvider? _provider;

    public IServiceProvider Services => _provider ?? throw new InvalidOperationException("SQL Server integration tests are not configured.");

    public async Task InitializeAsync()
    {
        var baseConnection = Environment.GetEnvironmentVariable(SqlServerFactAttribute.ConnectionVariable);
        if (string.IsNullOrWhiteSpace(baseConnection))
        {
            return;
        }

        var connection = new SqlConnectionStringBuilder(baseConnection)
        {
            InitialCatalog = $"CambridgeExamTests_{Guid.NewGuid():N}"
        }.ConnectionString;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connection,
                ["Seed:ApplyMigrations"] = "true",
                ["Seed:SampleData"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));
        services.AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<CambridgeDbContext>();
        _provider = services.BuildServiceProvider();

        await using var scope = _provider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DbInitializer>().InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        if (_provider is null)
        {
            return;
        }

        await using (var scope = _provider.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<CambridgeDbContext>().Database.EnsureDeletedAsync();
        }

        await _provider.DisposeAsync();
    }
}
