using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class TestAnswerConfiguration : IEntityTypeConfiguration<TestAnswer>
{
    public void Configure(EntityTypeBuilder<TestAnswer> builder)
    {
        builder.ToTable("AttemptAnswers");
        builder.HasKey(x => x.AttemptAnswerId);
        builder.Property(x => x.ScoreEarned).HasPrecision(10, 2);
        builder.Property(x => x.SavedAtUtc).IsUtcTimestamp(withDefault: true);

        builder.HasIndex(x => new { x.ExamAttemptId, x.QuestionId }).IsUnique().HasDatabaseName("UQ_AttemptAnswers_AttemptQuestion");

        builder.HasOne(x => x.TestAttempt).WithMany(x => x.Answers).HasForeignKey(x => x.ExamAttemptId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SelectedOption).WithMany().HasForeignKey(x => x.SelectedOptionId).OnDelete(DeleteBehavior.Restrict);
    }
}
