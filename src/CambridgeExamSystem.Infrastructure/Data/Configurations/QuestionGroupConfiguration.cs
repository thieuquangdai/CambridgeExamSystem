using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class QuestionGroupConfiguration : IEntityTypeConfiguration<QuestionGroup>
{
    public void Configure(EntityTypeBuilder<QuestionGroup> builder)
    {
        builder.ToTable("QuestionGroups");
        builder.HasKey(x => x.QuestionGroupId);
        builder.Property(x => x.GroupTitle).HasMaxLength(250);
        builder.HasIndex(x => new { x.ExamSectionId, x.GroupOrder }).IsUnique();
        builder.HasOne(x => x.Section).WithMany(x => x.QuestionGroups).HasForeignKey(x => x.ExamSectionId).OnDelete(DeleteBehavior.Restrict);
    }
}
