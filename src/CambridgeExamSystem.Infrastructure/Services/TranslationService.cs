using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CambridgeExamSystem.Infrastructure.Services;

public sealed class TranslationService(
    HttpClient httpClient,
    IMemoryCache cache,
    IOptions<TranslationOptions> options,
    ILogger<TranslationService> logger) : ITranslationService
{
    private readonly TranslationOptions _options = options.Value;

    public async Task<TranslationResultDto> TranslateAsync(TranslateRequest request, CancellationToken cancellationToken = default)
    {
        var text = request.Text.Trim();
        var source = request.SourceLanguage.Trim().ToLowerInvariant();
        var target = request.TargetLanguage.Trim().ToLowerInvariant();
        var result = new TranslationResultDto { OriginalText = text };

        if (!_options.Enabled)
        {
            result.ErrorMessage = "Dịch vụ dịch đang tắt.";
            return result;
        }

        var cacheKey = $"translate:{source}:{target}:{text.ToLowerInvariant()}";
        if (cache.TryGetValue(cacheKey, out TranslationResultDto? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            result.TranslatedText = await TranslateTextAsync(text, source, target, cancellationToken);
            if (source == "en" && IsSingleWord(text))
            {
                await FillDictionaryAsync(result, text, cancellationToken);
            }

            result.Success = !string.IsNullOrWhiteSpace(result.TranslatedText);
            if (!result.Success)
            {
                result.ErrorMessage = "Không tìm thấy bản dịch.";
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogWarning(ex, "Translation request failed for {Length} characters", text.Length);
            result.ErrorMessage = "Không kết nối được dịch vụ dịch. Vui lòng thử lại sau.";
            return result;
        }

        if (result.Success)
        {
            cache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheMinutes));
        }

        return result;
    }

    private async Task<string?> TranslateTextAsync(string text, string source, string target, CancellationToken cancellationToken)
    {
        var url = $"{_options.TranslateEndpoint}?q={Uri.EscapeDataString(text)}&langpair={Uri.EscapeDataString(source)}|{Uri.EscapeDataString(target)}";
        if (!string.IsNullOrWhiteSpace(_options.ContactEmail))
        {
            url += $"&de={Uri.EscapeDataString(_options.ContactEmail)}";
        }

        var response = await httpClient.GetFromJsonAsync<MyMemoryResponse>(url, cancellationToken);
        var translated = response?.ResponseData?.TranslatedText?.Trim();
        return response?.ResponseStatus == 200 && !string.IsNullOrWhiteSpace(translated) ? translated : null;
    }

    private async Task FillDictionaryAsync(TranslationResultDto result, string word, CancellationToken cancellationToken)
    {
        try
        {
            await LookupDictionaryAsync(result, word, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogInformation(ex, "Dictionary lookup failed; returning translation only");
        }
    }

    private async Task LookupDictionaryAsync(TranslationResultDto result, string word, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(_options.DictionaryEndpoint + Uri.EscapeDataString(word.ToLowerInvariant()), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var entries = await response.Content.ReadFromJsonAsync<List<DictionaryEntry>>(cancellationToken);
        var entry = entries?.FirstOrDefault();
        if (entry is null)
        {
            return;
        }

        var phonetic = entry.Phonetics?.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Audio)) ?? entry.Phonetics?.FirstOrDefault();
        result.Phonetic = entry.Phonetic ?? phonetic?.Text;
        result.AudioUrl = string.IsNullOrWhiteSpace(phonetic?.Audio) ? null : phonetic.Audio;
        var definition = entry.Meanings?.SelectMany(m => m.Definitions ?? []).FirstOrDefault();
        result.DefinitionText = definition?.Definition;
        result.ExampleText = definition?.Example;
    }

    private static bool IsSingleWord(string text) => text.Length <= 50 && text.All(c => char.IsLetter(c) || c is '-' or '\'');

    private sealed class MyMemoryResponse
    {
        [JsonPropertyName("responseData")]
        public MyMemoryData? ResponseData { get; set; }

        [JsonPropertyName("responseStatus")]
        public int ResponseStatus { get; set; }
    }

    private sealed class MyMemoryData
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }
    }

    private sealed class DictionaryEntry
    {
        [JsonPropertyName("phonetic")]
        public string? Phonetic { get; set; }

        [JsonPropertyName("phonetics")]
        public List<DictionaryPhonetic>? Phonetics { get; set; }

        [JsonPropertyName("meanings")]
        public List<DictionaryMeaning>? Meanings { get; set; }
    }

    private sealed class DictionaryPhonetic
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("audio")]
        public string? Audio { get; set; }
    }

    private sealed class DictionaryMeaning
    {
        [JsonPropertyName("definitions")]
        public List<DictionaryDefinition>? Definitions { get; set; }
    }

    private sealed class DictionaryDefinition
    {
        [JsonPropertyName("definition")]
        public string? Definition { get; set; }

        [JsonPropertyName("example")]
        public string? Example { get; set; }
    }
}
