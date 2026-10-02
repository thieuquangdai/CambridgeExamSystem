using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class ExplanationConfiguration : IEntityTypeConfiguration<Explanation>
{
    public void Configure(EntityTypeBuilder<Explanation> builder)
    {
        builder.ToTable("QuestionExplanations");
        builder.HasKey(x => x.ExplanationId);
        builder.Property(x => x.ExplanationTitle).HasMaxLength(250);
        builder.Property(x => x.ExplanationText).IsRequired();
        builder.Property(x => x.ExplanationMediaUrl).HasMaxLength(1000);
        builder.HasOne(x => x.Question).WithMany(x => x.Explanations).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Cascade);
    }
}
