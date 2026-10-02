namespace CambridgeExamSystem.Domain.Entities;

public class UserVocabulary
{
    public long UserVocabularyId { get; set; }
    public int UserId { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string? TranslatedText { get; set; }
    public string? DefinitionText { get; set; }
    public string? ExampleText { get; set; }
    public string? Phonetic { get; set; }
    public string? AudioUrl { get; set; }
    public int Frequency { get; set; } = 1;
    public bool IsBookmarked { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastViewedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
