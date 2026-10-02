using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CambridgeExamSystem.Infrastructure.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CambridgeDbContext>
{
    private const string DefaultConnection =
        "Server=(localdb)\\MSSQLLocalDB;Database=CambridgeExamDb;Trusted_Connection=True;TrustServerCertificate=True";

    public CambridgeDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CAMBRIDGE_DESIGN_CONNECTION") ?? DefaultConnection;
        var options = new DbContextOptionsBuilder<CambridgeDbContext>()
            .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(CambridgeDbContext).Assembly.FullName))
            .Options;
        return new CambridgeDbContext(options);
    }
}
