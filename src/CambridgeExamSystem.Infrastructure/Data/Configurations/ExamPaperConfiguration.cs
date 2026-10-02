using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class ExamPaperConfiguration : IEntityTypeConfiguration<ExamPaper>
{
    public void Configure(EntityTypeBuilder<ExamPaper> builder)
    {
        builder.ToTable("ExamPapers", t =>
        {
            t.HasCheckConstraint("CK_ExamPapers_Duration", "[DurationMinutes] > 0");
            t.HasCheckConstraint("CK_ExamPapers_TotalScore", "[TotalScore] >= 0");
        });
        builder.HasKey(x => x.ExamPaperId);
        builder.Property(x => x.ExamCode).IsVarchar(50).IsRequired();
        builder.Property(x => x.ExamName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.TotalScore).HasPrecision(10, 2);
        builder.Property(x => x.PassPercentage).HasPrecision(5, 2);
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.Property(x => x.UpdatedAtUtc).IsUtcTimestamp();

        builder.HasIndex(x => x.ExamCode).IsUnique();
        builder.HasIndex(x => new { x.LevelId, x.IsPublished, x.IsActive }).HasDatabaseName("IX_ExamPapers_Level_Published");

        builder.HasOne(x => x.Level).WithMany(x => x.ExamPapers).HasForeignKey(x => x.LevelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
