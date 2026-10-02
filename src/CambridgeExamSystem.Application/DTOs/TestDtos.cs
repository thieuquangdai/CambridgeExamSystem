namespace CambridgeExamSystem.Application.DTOs;

public sealed class TakeTestDto
{
    public long ExamAttemptId { get; set; }
    public int ExamPaperId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int RemainingSeconds { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public DateTime LastSavedAtUtc { get; set; }
    public bool IsResumed { get; set; }
    public List<TestSectionDto> Sections { get; set; } = [];
    public List<SavedAnswerDto> SavedAnswers { get; set; } = [];

    public int TotalQuestions => Sections.Sum(s => s.Questions.Count);
}

public sealed class TestSectionDto
{
    public int ExamSectionId { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? SectionDescription { get; set; }
    public List<TestGroupDto> Groups { get; set; } = [];
    public List<TestQuestionDto> Questions { get; set; } = [];
}

public sealed class TestGroupDto
{
    public int QuestionGroupId { get; set; }
    public string? GroupTitle { get; set; }
    public string? Instructions { get; set; }
    public string? PassageText { get; set; }
    public List<MediaDto> Media { get; set; } = [];
}

public sealed class TestQuestionDto
{
    public int QuestionId { get; set; }
    public int? QuestionGroupId { get; set; }
    public int Index { get; set; }
    public int QuestionOrder { get; set; }
    public string? QuestionCode { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? InstructionText { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public List<TestOptionDto> Options { get; set; } = [];
    public List<string> MatchingKeys { get; set; } = [];
    public List<MediaDto> Media { get; set; } = [];
}

public sealed class TestOptionDto
{
    public int QuestionOptionId { get; set; }
    public string? OptionCode { get; set; }
    public string OptionText { get; set; } = string.Empty;
}

public sealed class MediaDto
{
    public int MediaId { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public string? AltText { get; set; }
}

public sealed class SavedAnswerDto
{
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }
    public string? TextAnswer { get; set; }
    public string? AnswerJson { get; set; }
    public bool IsAnswered { get; set; }
}

public sealed class SaveAnswerRequest
{
    public long ExamAttemptId { get; set; }
    public int? QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }
    public string? TextAnswer { get; set; }
    public string? AnswerJson { get; set; }
    public int TimeSpentSeconds { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int? CurrentQuestionId { get; set; }
}

public sealed class SaveAnswerResult
{
    public bool Success { get; set; }
    public DateTime SavedAtUtc { get; set; }
    public int RemainingSeconds { get; set; }
    public bool IsExpired { get; set; }
    public string? Message { get; set; }
}

public sealed class ActiveAttemptDto
{
    public long ExamAttemptId { get; set; }
    public int ExamPaperId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public DateTime LastSavedAtUtc { get; set; }
    public int AnsweredCount { get; set; }
    public int RemainingSeconds { get; set; }
}
