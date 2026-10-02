using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions", t => t.HasCheckConstraint("CK_Questions_Score", "[Score] >= 0"));
        builder.HasKey(x => x.QuestionId);
        builder.Property(x => x.QuestionCode).IsVarchar(50);
        builder.Property(x => x.QuestionText).IsRequired();
        builder.Property(x => x.Score).HasPrecision(10, 2);
        builder.Property(x => x.DifficultyCode).IsVarchar(30);
        builder.Property(x => x.CorrectTextAnswer).HasMaxLength(2000);

        builder.HasIndex(x => new { x.ExamSectionId, x.QuestionOrder }).IsUnique().HasDatabaseName("IX_Questions_Section_Order");

        builder.HasOne(x => x.Section).WithMany(x => x.Questions).HasForeignKey(x => x.ExamSectionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.QuestionGroup).WithMany(x => x.Questions).HasForeignKey(x => x.QuestionGroupId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.QuestionType).WithMany(x => x.Questions).HasForeignKey(x => x.QuestionTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
