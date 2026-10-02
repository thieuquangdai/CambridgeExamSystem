using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Domain.Entities;

public class StarTransaction
{
    public long StarTransactionId { get; set; }
    public int UserId { get; set; }
    public long? ExamAttemptId { get; set; }
    public int Amount { get; set; }
    public StarTransactionType TransactionType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public TestAttempt? TestAttempt { get; set; }
}
