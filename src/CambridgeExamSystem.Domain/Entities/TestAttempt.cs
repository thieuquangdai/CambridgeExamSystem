using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Domain.Entities;

public class TestAttempt
{
    public long ExamAttemptId { get; set; }
    public int UserId { get; set; }
    public int ExamPaperId { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
    public DateTime StartedAtUtc { get; set; }
    public DateTime LastSavedAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public int? CurrentQuestionId { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TotalSecondsUsed { get; set; }
    public decimal? TotalScore { get; set; }
    public decimal? MaxScore { get; set; }
    public int? CorrectCount { get; set; }
    public int? WrongCount { get; set; }
    public int? UnansweredCount { get; set; }
    public decimal? PercentageScore { get; set; }
    public bool IsSubmitted { get; set; }

    public User User { get; set; } = null!;
    public ExamPaper ExamPaper { get; set; } = null!;
    public Question? CurrentQuestion { get; set; }
    public AttemptResult? Result { get; set; }
    public ICollection<TestAnswer> Answers { get; set; } = new List<TestAnswer>();
    public ICollection<AttemptEvent> Events { get; set; } = new List<AttemptEvent>();
    public ICollection<StarTransaction> StarTransactions { get; set; } = new List<StarTransaction>();

    public bool IsOpen => !IsSubmitted && Status is AttemptStatus.InProgress or AttemptStatus.Paused;
}
