using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class UserStatisticConfiguration : IEntityTypeConfiguration<UserStatistic>
{
    public void Configure(EntityTypeBuilder<UserStatistic> builder)
    {
        builder.ToTable("UserStatistics");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();
        builder.Property(x => x.AverageScore).HasPrecision(5, 2);
        builder.Property(x => x.BestPercentage).HasPrecision(5, 2);
        builder.Property(x => x.CurrentRankCode).IsVarchar(30).HasDefaultValue("Bronze");
        builder.Property(x => x.UpdatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.HasOne(x => x.User).WithOne(x => x.Statistics).HasForeignKey<UserStatistic>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
