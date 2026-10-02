using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Domain.Entities;

public class QuestionMedia
{
    public int MediaId { get; set; }
    public int? QuestionId { get; set; }
    public int? QuestionGroupId { get; set; }
    public MediaType MediaType { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSizeBytes { get; set; }
    public int DisplayOrder { get; set; }
    public int? DurationSeconds { get; set; }
    public string? AltText { get; set; }
    public bool IsActive { get; set; } = true;

    public Question? Question { get; set; }
    public QuestionGroup? QuestionGroup { get; set; }
}
