using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class LeaderboardSnapshotConfiguration : IEntityTypeConfiguration<LeaderboardSnapshot>
{
    public void Configure(EntityTypeBuilder<LeaderboardSnapshot> builder)
    {
        builder.ToTable("LeaderboardSnapshots", t =>
            t.HasCheckConstraint("CK_LeaderboardSnapshots_PeriodType", "[PeriodType] IN ('Weekly', 'Monthly', 'AllTime')"));
        builder.HasKey(x => x.LeaderboardSnapshotId);
        builder.Property(x => x.PeriodType).HasConversion<string>().IsVarchar(20);
        builder.Property(x => x.AverageScore).HasPrecision(5, 2);
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.HasIndex(x => new { x.PeriodType, x.PeriodStartDate, x.LevelId, x.RankNumber }).HasDatabaseName("IX_Leaderboard_Period");
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Level).WithMany().HasForeignKey(x => x.LevelId).OnDelete(DeleteBehavior.Restrict);
    }
}
