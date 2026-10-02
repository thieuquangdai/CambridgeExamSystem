using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class AttemptResultConfiguration : IEntityTypeConfiguration<AttemptResult>
{
    public void Configure(EntityTypeBuilder<AttemptResult> builder)
    {
        builder.ToTable("AttemptResults", t => t.HasCheckConstraint("CK_AttemptResults_Percentage", "[PercentageScore] BETWEEN 0 AND 100"));
        builder.HasKey(x => x.AttemptResultId);
        builder.Property(x => x.TotalScore).HasPrecision(10, 2);
        builder.Property(x => x.MaxScore).HasPrecision(10, 2);
        builder.Property(x => x.PercentageScore).HasPrecision(5, 2);
        builder.Property(x => x.ResultMessage).HasMaxLength(1000);
        builder.Property(x => x.GradedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.HasIndex(x => x.ExamAttemptId).IsUnique();
        builder.HasOne(x => x.TestAttempt).WithOne(x => x.Result).HasForeignKey<AttemptResult>(x => x.ExamAttemptId).OnDelete(DeleteBehavior.Cascade);
    }
}
