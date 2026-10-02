namespace CambridgeExamSystem.Domain.Entities;

public class AttemptEvent
{
    public long AttemptEventId { get; set; }
    public long ExamAttemptId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? EventDataJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public TestAttempt TestAttempt { get; set; } = null!;
}
