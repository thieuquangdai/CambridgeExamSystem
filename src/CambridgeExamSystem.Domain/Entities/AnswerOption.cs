namespace CambridgeExamSystem.Domain.Entities;

public class AnswerOption
{
    public int QuestionOptionId { get; set; }
    public int QuestionId { get; set; }
    public string? OptionCode { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int OptionOrder { get; set; }
    public bool IsCorrect { get; set; }
    public string? MatchingKey { get; set; }

    public Question Question { get; set; } = null!;
}
