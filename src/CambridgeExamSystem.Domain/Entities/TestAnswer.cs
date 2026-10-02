namespace CambridgeExamSystem.Domain.Entities;

public class TestAnswer
{
    public long AttemptAnswerId { get; set; }
    public long ExamAttemptId { get; set; }
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }
    public string? TextAnswer { get; set; }
    public string? AnswerJson { get; set; }
    public bool IsAnswered { get; set; }
    public bool? IsCorrect { get; set; }
    public decimal? ScoreEarned { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime SavedAtUtc { get; set; }

    public TestAttempt TestAttempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public AnswerOption? SelectedOption { get; set; }
}
