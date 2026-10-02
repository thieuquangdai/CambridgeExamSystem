namespace CambridgeExamSystem.Infrastructure.Services;

public sealed class TranslationOptions
{
    public const string SectionName = "Translation";

    public bool Enabled { get; set; } = true;
    public string TranslateEndpoint { get; set; } = "https://api.mymemory.translated.net/get";
    public string DictionaryEndpoint { get; set; } = "https://api.dictionaryapi.dev/api/v2/entries/en/";
    public string? ContactEmail { get; set; }
    public int TimeoutSeconds { get; set; } = 8;
    public int CacheMinutes { get; set; } = 720;
}
