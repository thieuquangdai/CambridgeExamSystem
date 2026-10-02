namespace CambridgeExamSystem.Application.DTOs;

public sealed class ExamPaperEditDto
{
    public int ExamPaperId { get; set; }
    public int LevelId { get; set; }
    public string ExamCode { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public decimal? PassPercentage { get; set; } = 60;
    public bool IsPublished { get; set; }
}

public sealed class SectionEditDto
{
    public int ExamPaperId { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? SectionDescription { get; set; }
    public int? DurationMinutes { get; set; }
}

public sealed class QuestionEditDto
{
    public int ExamSectionId { get; set; }
    public int QuestionTypeId { get; set; }
    public string? QuestionCode { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? InstructionText { get; set; }
    public decimal Score { get; set; } = 1;
    public string? DifficultyCode { get; set; }
    public string? CorrectTextAnswer { get; set; }
    public string? ExplanationText { get; set; }
    public string? TranscriptText { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public List<OptionEditDto> Options { get; set; } = [];
}

public sealed class OptionEditDto
{
    public string? OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public string? MatchingKey { get; set; }
}

public sealed class ExamContentDto
{
    public ExamPaperSummaryDto Exam { get; set; } = new();
    public List<SectionContentDto> Sections { get; set; } = [];
}

public sealed class SectionContentDto
{
    public int ExamSectionId { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int SectionOrder { get; set; }
    public decimal MaxScore { get; set; }
    public List<QuestionAdminDto> Questions { get; set; } = [];
}

public sealed class QuestionAdminDto
{
    public int QuestionId { get; set; }
    public int QuestionOrder { get; set; }
    public string? QuestionCode { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? CorrectTextAnswer { get; set; }
    public List<OptionAdminDto> Options { get; set; } = [];
}

public sealed class OptionAdminDto
{
    public int QuestionOptionId { get; set; }
    public string? OptionCode { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string? MatchingKey { get; set; }
}

public sealed class AdminDashboardDto
{
    public int TotalExams { get; set; }
    public int PublishedExams { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalAttempts { get; set; }
    public int SubmittedAttempts { get; set; }
    public decimal AveragePercentage { get; set; }
}
