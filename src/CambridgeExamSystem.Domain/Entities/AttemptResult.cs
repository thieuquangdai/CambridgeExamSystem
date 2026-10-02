namespace CambridgeExamSystem.Domain.Entities;

public class AttemptResult
{
    public long AttemptResultId { get; set; }
    public long ExamAttemptId { get; set; }
    public decimal TotalScore { get; set; }
    public decimal MaxScore { get; set; }
    public decimal PercentageScore { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public int UnansweredCount { get; set; }
    public int StarsEarned { get; set; }
    public string? ResultMessage { get; set; }
    public DateTime GradedAtUtc { get; set; }

    public TestAttempt TestAttempt { get; set; } = null!;
}
