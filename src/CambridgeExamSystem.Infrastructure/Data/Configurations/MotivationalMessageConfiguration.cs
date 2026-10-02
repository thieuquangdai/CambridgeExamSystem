using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class MotivationalMessageConfiguration : IEntityTypeConfiguration<MotivationalMessage>
{
    public void Configure(EntityTypeBuilder<MotivationalMessage> builder)
    {
        builder.ToTable("MotivationalMessages");
        builder.HasKey(x => x.MotivationalMessageId);
        builder.Property(x => x.MessageType).IsVarchar(30).IsRequired();
        builder.Property(x => x.MessageText).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.MinPercentage).HasPrecision(5, 2);
        builder.Property(x => x.MaxPercentage).HasPrecision(5, 2);
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
    }
}
