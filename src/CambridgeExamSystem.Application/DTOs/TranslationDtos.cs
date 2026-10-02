namespace CambridgeExamSystem.Application.DTOs;

public sealed class TranslateRequest
{
    public string Text { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = "en";
    public string TargetLanguage { get; set; } = "vi";
}

public sealed class TranslationResultDto
{
    public string OriginalText { get; set; } = string.Empty;
    public string? TranslatedText { get; set; }
    public string? Phonetic { get; set; }
    public string? DefinitionText { get; set; }
    public string? ExampleText { get; set; }
    public string? AudioUrl { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class SaveVocabularyRequest
{
    public string OriginalText { get; set; } = string.Empty;
    public string? TranslatedText { get; set; }
    public string? DefinitionText { get; set; }
    public string? ExampleText { get; set; }
    public string? Phonetic { get; set; }
    public string? AudioUrl { get; set; }
}

public sealed class VocabularyDto
{
    public long UserVocabularyId { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string? TranslatedText { get; set; }
    public string? DefinitionText { get; set; }
    public string? ExampleText { get; set; }
    public string? Phonetic { get; set; }
    public string? AudioUrl { get; set; }
    public int Frequency { get; set; }
    public bool IsBookmarked { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastViewedAtUtc { get; set; }
}
