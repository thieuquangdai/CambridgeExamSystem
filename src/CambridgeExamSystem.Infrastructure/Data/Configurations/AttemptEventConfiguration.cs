using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class AttemptEventConfiguration : IEntityTypeConfiguration<AttemptEvent>
{
    public void Configure(EntityTypeBuilder<AttemptEvent> builder)
    {
        builder.ToTable("AttemptEvents");
        builder.HasKey(x => x.AttemptEventId);
        builder.Property(x => x.EventType).IsVarchar(50).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.HasOne(x => x.TestAttempt).WithMany(x => x.Events).HasForeignKey(x => x.ExamAttemptId).OnDelete(DeleteBehavior.Cascade);
    }
}
