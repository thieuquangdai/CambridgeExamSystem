using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("ExamSections");
        builder.HasKey(x => x.ExamSectionId);
        builder.Property(x => x.SectionCode).IsVarchar(30).IsRequired();
        builder.Property(x => x.SectionName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MaxScore).HasPrecision(10, 2);

        builder.HasIndex(x => new { x.ExamPaperId, x.SectionCode }).IsUnique();
        builder.HasIndex(x => new { x.ExamPaperId, x.SectionOrder }).IsUnique();

        builder.HasOne(x => x.ExamPaper).WithMany(x => x.Sections).HasForeignKey(x => x.ExamPaperId).OnDelete(DeleteBehavior.Restrict);
    }
}
