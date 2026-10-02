using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class UserVocabularyConfiguration : IEntityTypeConfiguration<UserVocabulary>
{
    public void Configure(EntityTypeBuilder<UserVocabulary> builder)
    {
        builder.ToTable("UserVocabulary");
        builder.HasKey(x => x.UserVocabularyId);
        builder.Property(x => x.OriginalText).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.TranslatedText).HasMaxLength(1000);
        builder.Property(x => x.Phonetic).HasMaxLength(200);
        builder.Property(x => x.AudioUrl).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.Property(x => x.LastViewedAtUtc).IsUtcTimestamp();
        builder.HasIndex(x => new { x.UserId, x.OriginalText }).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.Vocabulary).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
