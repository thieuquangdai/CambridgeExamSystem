using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("UserAchievements");
        builder.HasKey(x => x.UserAchievementId);
        builder.Property(x => x.EarnedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.Achievements).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Achievement).WithMany(x => x.UserAchievements).HasForeignKey(x => x.AchievementId).OnDelete(DeleteBehavior.Restrict);
    }
}
