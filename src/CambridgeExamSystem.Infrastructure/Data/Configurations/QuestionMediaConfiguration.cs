using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class QuestionMediaConfiguration : IEntityTypeConfiguration<QuestionMedia>
{
    public void Configure(EntityTypeBuilder<QuestionMedia> builder)
    {
        builder.ToTable("QuestionMedia", t =>
        {
            t.HasCheckConstraint("CK_QuestionMedia_Type", "[MediaType] IN ('Image', 'Audio', 'Video', 'Document')");
            t.HasCheckConstraint("CK_QuestionMedia_Owner", "[QuestionId] IS NOT NULL OR [QuestionGroupId] IS NOT NULL");
        });
        builder.HasKey(x => x.MediaId);
        builder.Property(x => x.MediaType).HasConversion<string>().IsVarchar(20);
        builder.Property(x => x.FileUrl).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(255);
        builder.Property(x => x.MimeType).IsVarchar(100);
        builder.Property(x => x.AltText).HasMaxLength(500);

        builder.HasIndex(x => new { x.QuestionId, x.DisplayOrder }).HasDatabaseName("IX_QuestionMedia_Question");

        builder.HasOne(x => x.Question).WithMany(x => x.Media).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.QuestionGroup).WithMany(x => x.Media).HasForeignKey(x => x.QuestionGroupId).OnDelete(DeleteBehavior.Restrict);
    }
}
