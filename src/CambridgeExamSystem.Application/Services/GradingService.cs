using System.Text.RegularExpressions;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Services;

public sealed record GradeOutcome(bool IsAnswered, bool IsCorrect, decimal ScoreEarned);

public interface IGradingService
{
    bool IsAnswered(int? selectedOptionId, string? textAnswer, string? answerJson);
    GradeOutcome Grade(Question question, TestAnswer? answer);
    string? DescribeAnswer(Question question, TestAnswer? answer);
    string? DescribeCorrectAnswer(Question question);
}

public sealed partial class GradingService : IGradingService
{
    public bool IsAnswered(int? selectedOptionId, string? textAnswer, string? answerJson) =>
        selectedOptionId is not null
        || !string.IsNullOrWhiteSpace(textAnswer)
        || AnswerPayload.Parse(answerJson)?.HasContent == true;

    public GradeOutcome Grade(Question question, TestAnswer? answer)
    {
        if (answer is null || !IsAnswered(answer.SelectedOptionId, answer.TextAnswer, answer.AnswerJson))
        {
            return new GradeOutcome(false, false, 0);
        }

        var isCorrect = question.QuestionType.TypeCode switch
        {
            QuestionTypeCodes.MultipleChoice or QuestionTypeCodes.TrueFalse => IsSingleChoiceCorrect(question, answer),
            QuestionTypeCodes.MultipleSelect => IsMultipleSelectCorrect(question, answer),
            QuestionTypeCodes.FillBlank or QuestionTypeCodes.ShortAnswer => IsTextCorrect(question, answer.TextAnswer),
            QuestionTypeCodes.Matching => IsMatchingCorrect(question, answer),
            QuestionTypeCodes.Ordering => IsOrderingCorrect(question, answer),
            _ => false
        };

        return new GradeOutcome(true, isCorrect, isCorrect ? question.Score : 0);
    }

    public string? DescribeAnswer(Question question, TestAnswer? answer)
    {
        if (answer is null || !IsAnswered(answer.SelectedOptionId, answer.TextAnswer, answer.AnswerJson))
        {
            return null;
        }

        var options = question.Options.ToDictionary(o => o.QuestionOptionId);
        var payload = AnswerPayload.Parse(answer.AnswerJson);

        switch (question.QuestionType.TypeCode)
        {
            case QuestionTypeCodes.FillBlank:
            case QuestionTypeCodes.ShortAnswer:
                return answer.TextAnswer?.Trim();
            case QuestionTypeCodes.MultipleSelect:
                return JoinOptionTexts(payload?.SelectedOptionIds, options, ", ");
            case QuestionTypeCodes.Ordering:
                return JoinOptionTexts(payload?.OrderedOptionIds, options, " → ");
            case QuestionTypeCodes.Matching:
                if (payload?.Matches is null)
                {
                    return null;
                }

                return string.Join("; ", question.Options
                    .OrderBy(o => o.OptionOrder)
                    .Select(o => $"{o.OptionText} → {(payload.Matches.TryGetValue(o.QuestionOptionId.ToString(), out var key) && !string.IsNullOrWhiteSpace(key) ? key : "?")}"));
            default:
                return answer.SelectedOptionId is int id && options.TryGetValue(id, out var option)
                    ? FormatOption(option)
                    : null;
        }
    }

    public string? DescribeCorrectAnswer(Question question)
    {
        var ordered = question.Options.OrderBy(o => o.OptionOrder).ToList();

        return question.QuestionType.TypeCode switch
        {
            QuestionTypeCodes.FillBlank or QuestionTypeCodes.ShortAnswer =>
                question.CorrectTextAnswer?.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
            QuestionTypeCodes.Ordering => string.Join(" → ", ordered.Select(o => o.OptionText)),
            QuestionTypeCodes.Matching => string.Join("; ", ordered.Select(o => $"{o.OptionText} → {o.MatchingKey}")),
            _ => string.Join(", ", ordered.Where(o => o.IsCorrect).Select(FormatOption))
        };
    }

    private static bool IsSingleChoiceCorrect(Question question, TestAnswer answer) =>
        answer.SelectedOptionId is int id && question.Options.Any(o => o.QuestionOptionId == id && o.IsCorrect);

    private static bool IsMultipleSelectCorrect(Question question, TestAnswer answer)
    {
        var selected = AnswerPayload.Parse(answer.AnswerJson)?.SelectedOptionIds?.ToHashSet() ?? [];
        var correct = question.Options.Where(o => o.IsCorrect).Select(o => o.QuestionOptionId).ToHashSet();
        return correct.Count > 0 && selected.SetEquals(correct);
    }

    private static bool IsTextCorrect(Question question, string? textAnswer)
    {
        if (string.IsNullOrWhiteSpace(question.CorrectTextAnswer) || string.IsNullOrWhiteSpace(textAnswer))
        {
            return false;
        }

        var normalized = NormalizeText(textAnswer);
        return question.CorrectTextAnswer
            .Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(accepted => NormalizeText(accepted) == normalized);
    }

    private static bool IsMatchingCorrect(Question question, TestAnswer answer)
    {
        var matches = AnswerPayload.Parse(answer.AnswerJson)?.Matches;
        var targets = question.Options.Where(o => !string.IsNullOrWhiteSpace(o.MatchingKey)).ToList();
        if (matches is null || targets.Count == 0)
        {
            return false;
        }

        return targets.All(o =>
            matches.TryGetValue(o.QuestionOptionId.ToString(), out var key)
            && string.Equals(key?.Trim(), o.MatchingKey!.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsOrderingCorrect(Question question, TestAnswer answer)
    {
        var ordered = AnswerPayload.Parse(answer.AnswerJson)?.OrderedOptionIds;
        var expected = question.Options.OrderBy(o => o.OptionOrder).Select(o => o.QuestionOptionId).ToList();
        return ordered is not null && expected.Count > 0 && ordered.SequenceEqual(expected);
    }

    private static string NormalizeText(string value)
    {
        var collapsed = WhitespaceRegex().Replace(value.Trim(), " ");
        return collapsed.TrimEnd('.', '!', '?', ',', ';').ToLowerInvariant();
    }

    private static string FormatOption(AnswerOption option) =>
        string.IsNullOrWhiteSpace(option.OptionCode) ? option.OptionText : $"{option.OptionCode}. {option.OptionText}";

    private static string? JoinOptionTexts(IEnumerable<int>? ids, IReadOnlyDictionary<int, AnswerOption> options, string separator)
    {
        if (ids is null)
        {
            return null;
        }

        var texts = ids.Where(options.ContainsKey).Select(id => options[id].OptionText).ToList();
        return texts.Count == 0 ? null : string.Join(separator, texts);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
