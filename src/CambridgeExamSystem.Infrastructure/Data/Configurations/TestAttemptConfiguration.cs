using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder.ToTable("ExamAttempts", t =>
        {
            t.HasCheckConstraint("CK_ExamAttempts_Status", "[Status] IN ('InProgress', 'Paused', 'Submitted', 'Expired', 'Cancelled')");
            t.HasCheckConstraint("CK_ExamAttempts_Score", "[TotalScore] IS NULL OR [TotalScore] >= 0");
        });
        builder.HasKey(x => x.ExamAttemptId);
        builder.Ignore(x => x.IsOpen);
        builder.Property(x => x.Status).HasConversion<string>().IsVarchar(30);
        builder.Property(x => x.StartedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.Property(x => x.LastSavedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.Property(x => x.SubmittedAtUtc).IsUtcTimestamp();
        builder.Property(x => x.TotalScore).HasPrecision(10, 2);
        builder.Property(x => x.MaxScore).HasPrecision(10, 2);
        builder.Property(x => x.PercentageScore).HasPrecision(5, 2);

        builder.HasIndex(x => new { x.UserId, x.Status, x.LastSavedAtUtc }).IsDescending(false, false, true).HasDatabaseName("IX_ExamAttempts_User_Status");
        builder.HasIndex(x => new { x.UserId, x.ExamPaperId, x.StartedAtUtc }).IsDescending(false, false, true).HasDatabaseName("IX_ExamAttempts_User_Exam");

        builder.HasOne(x => x.User).WithMany(x => x.TestAttempts).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ExamPaper).WithMany(x => x.TestAttempts).HasForeignKey(x => x.ExamPaperId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CurrentQuestion).WithMany().HasForeignKey(x => x.CurrentQuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
