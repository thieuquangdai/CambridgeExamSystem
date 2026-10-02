namespace CambridgeExamSystem.Domain.Entities;

public class Explanation
{
    public int ExplanationId { get; set; }
    public int QuestionId { get; set; }
    public string? ExplanationTitle { get; set; }
    public string ExplanationText { get; set; } = string.Empty;
    public string? TranscriptText { get; set; }
    public string? GrammarNote { get; set; }
    public string? VocabularyNote { get; set; }
    public string? ExplanationMediaUrl { get; set; }
    public bool IsPublished { get; set; } = true;

    public Question Question { get; set; } = null!;
}
