using System.Text.Json;

namespace CambridgeExamSystem.Application.Services;

public sealed class AnswerPayload
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public List<int>? SelectedOptionIds { get; set; }
    public List<int>? OrderedOptionIds { get; set; }
    public Dictionary<string, string>? Matches { get; set; }

    public bool HasContent =>
        SelectedOptionIds is { Count: > 0 }
        || OrderedOptionIds is { Count: > 0 }
        || (Matches is not null && Matches.Values.Any(v => !string.IsNullOrWhiteSpace(v)));

    public static AnswerPayload? Parse(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AnswerPayload>(json, SerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
