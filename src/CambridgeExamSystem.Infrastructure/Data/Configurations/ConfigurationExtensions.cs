using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

internal static class ConfigurationExtensions
{
    public const string UtcNowSql = "SYSUTCDATETIME()";

    public static PropertyBuilder<DateTime> IsUtcTimestamp(this PropertyBuilder<DateTime> builder, bool withDefault = false)
    {
        builder.HasColumnType("datetime2(0)");
        if (withDefault)
        {
            builder.HasDefaultValueSql(UtcNowSql);
        }

        return builder;
    }

    public static PropertyBuilder<DateTime?> IsUtcTimestamp(this PropertyBuilder<DateTime?> builder) =>
        builder.HasColumnType("datetime2(0)");

    public static PropertyBuilder<TProperty> IsVarchar<TProperty>(this PropertyBuilder<TProperty> builder, int maxLength) =>
        builder.IsUnicode(false).HasMaxLength(maxLength);
}
