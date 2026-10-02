using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class AnswerOptionConfiguration : IEntityTypeConfiguration<AnswerOption>
{
    public void Configure(EntityTypeBuilder<AnswerOption> builder)
    {
        builder.ToTable("QuestionOptions");
        builder.HasKey(x => x.QuestionOptionId);
        builder.Property(x => x.OptionCode).IsVarchar(10);
        builder.Property(x => x.OptionText).IsRequired();
        builder.Property(x => x.MatchingKey).IsVarchar(100);
        builder.HasIndex(x => new { x.QuestionId, x.OptionOrder }).IsUnique();
        builder.HasOne(x => x.Question).WithMany(x => x.Options).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Cascade);
    }
}
