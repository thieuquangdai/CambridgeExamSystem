namespace CambridgeExamSystem.Tests.Infrastructure;

public sealed class SqlServerFactAttribute : FactAttribute
{
    public const string ConnectionVariable = "CAMBRIDGE_TEST_SQLSERVER";

    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable)))
        {
            Skip = $"Set {ConnectionVariable} to a SQL Server connection string to run database integration tests.";
        }
    }
}
