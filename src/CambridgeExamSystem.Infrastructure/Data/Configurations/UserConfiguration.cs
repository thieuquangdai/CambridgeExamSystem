using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(x => x.Id).HasColumnName("UserId");
        builder.Property(x => x.UserName).HasMaxLength(100);
        builder.Property(x => x.NormalizedUserName).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(255);
        builder.Property(x => x.NormalizedEmail).HasMaxLength(255);
        builder.Property(x => x.PasswordHash).HasMaxLength(500);
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.AvatarUrl).HasMaxLength(500);
        builder.Property(x => x.CurrentLevelCode).IsVarchar(30);
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.Property(x => x.UpdatedAtUtc).IsUtcTimestamp();
        builder.Property(x => x.LastLoginAtUtc).IsUtcTimestamp();

        builder.HasIndex(x => x.NormalizedEmail).IsUnique().HasFilter("[NormalizedEmail] IS NOT NULL");
    }
}
