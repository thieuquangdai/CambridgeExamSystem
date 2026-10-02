namespace CambridgeExamSystem.Application.DTOs;

public sealed class AttemptResultDto
{
    public long ExamAttemptId { get; set; }
    public int ExamPaperId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public int TotalSecondsUsed { get; set; }
    public decimal TotalScore { get; set; }
    public decimal MaxScore { get; set; }
    public decimal PercentageScore { get; set; }
    public decimal? PassPercentage { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public int UnansweredCount { get; set; }
    public int StarsEarned { get; set; }
    public string? ResultMessage { get; set; }
    public List<AchievementDto> NewAchievements { get; set; } = [];
    public List<QuestionResultDto> Questions { get; set; } = [];
    public List<SectionResultDto> Sections { get; set; } = [];

    public bool IsPassed => PassPercentage is null || PercentageScore >= PassPercentage;
}

public sealed class SectionResultDto
{
    public string SectionName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public int CorrectCount { get; set; }
    public int QuestionCount { get; set; }
}

public sealed class QuestionResultDto
{
    public int QuestionId { get; set; }
    public int Index { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string TypeCode { get; set; } = string.Empty;
    public string? UserAnswerText { get; set; }
    public string? CorrectAnswerText { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsCorrect { get; set; }
    public decimal ScoreEarned { get; set; }
    public decimal MaxScore { get; set; }
    public string? ExplanationText { get; set; }
    public string? TranscriptText { get; set; }
    public string? GrammarNote { get; set; }
    public string? VocabularyNote { get; set; }
}

public sealed class AttemptHistoryItemDto
{
    public long ExamAttemptId { get; set; }
    public int ExamPaperId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public int AttemptNumber { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public decimal? PercentageScore { get; set; }
    public decimal? TotalScore { get; set; }
    public decimal? MaxScore { get; set; }
    public int StarsEarned { get; set; }
    public string Status { get; set; } = string.Empty;
}
