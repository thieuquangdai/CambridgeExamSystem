using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CambridgeExamSystem.Infrastructure.Data.Configurations;

public sealed class StarTransactionConfiguration : IEntityTypeConfiguration<StarTransaction>
{
    public void Configure(EntityTypeBuilder<StarTransaction> builder)
    {
        builder.ToTable("StarTransactions");
        builder.HasKey(x => x.StarTransactionId);
        builder.Property(x => x.TransactionType).HasConversion<string>().IsVarchar(50);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsUtcTimestamp(withDefault: true);
        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc }).IsDescending(false, true).HasDatabaseName("IX_StarTransactions_User_Date");
        builder.HasOne(x => x.User).WithMany(x => x.StarTransactions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TestAttempt).WithMany(x => x.StarTransactions).HasForeignKey(x => x.ExamAttemptId).OnDelete(DeleteBehavior.Restrict);
    }
}
