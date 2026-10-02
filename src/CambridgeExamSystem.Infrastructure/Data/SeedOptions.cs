namespace CambridgeExamSystem.Infrastructure.Data;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public bool ApplyMigrations { get; set; } = true;
    public bool SampleData { get; set; } = true;
    public string? AdminEmail { get; set; }
    public string? AdminPassword { get; set; }
    public string AdminFullName { get; set; } = "System Administrator";
}
