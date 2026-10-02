namespace CambridgeExamSystem.Application.DTOs;

public sealed class ExamLevelDto
{
    public int LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int PublishedExamCount { get; set; }
}

public sealed class ExamPaperSummaryDto
{
    public int ExamPaperId { get; set; }
    public string ExamCode { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal TotalScore { get; set; }
    public decimal? PassPercentage { get; set; }
    public bool IsPublished { get; set; }
    public bool IsActive { get; set; }
    public int SectionCount { get; set; }
    public int QuestionCount { get; set; }
    public long? ActiveAttemptId { get; set; }
    public decimal? BestPercentage { get; set; }
    public int AttemptCount { get; set; }
}

public sealed class ExamPaperDetailDto
{
    public ExamPaperSummaryDto Summary { get; set; } = new();
    public List<SectionSummaryDto> Sections { get; set; } = [];
    public List<AttemptHistoryItemDto> History { get; set; } = [];
}

public sealed class SectionSummaryDto
{
    public int ExamSectionId { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? SectionDescription { get; set; }
    public int SectionOrder { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal MaxScore { get; set; }
    public int QuestionCount { get; set; }
}

public sealed class QuestionTypeDto
{
    public int QuestionTypeId { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public bool RequiresOptions { get; set; }
    public bool AllowsTextAnswer { get; set; }
}
