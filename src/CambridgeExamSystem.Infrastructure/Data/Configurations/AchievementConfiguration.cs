using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievements");
        builder.HasKey(x => x.AchievementId);
        builder.Property(x => x.AchievementCode).IsVarchar(50).IsRequired();
        builder.Property(x => x.AchievementName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.IconUrl).HasMaxLength(500);
        builder.Property(x => x.BadgeColor).IsVarchar(30);
        builder.Property(x => x.RequiredScore).HasPrecision(10, 2);
        builder.HasIndex(x => x.AchievementCode).IsUnique();
    }
}
