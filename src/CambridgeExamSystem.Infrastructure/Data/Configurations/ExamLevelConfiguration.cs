using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class ExamLevelConfiguration : IEntityTypeConfiguration<ExamLevel>
{
    public void Configure(EntityTypeBuilder<ExamLevel> builder)
    {
        builder.ToTable("ExamLevels");
        builder.HasKey(x => x.LevelId);
        builder.Property(x => x.LevelCode).IsVarchar(30).IsRequired();
        builder.Property(x => x.LevelName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => x.LevelCode).IsUnique();
    }
}
