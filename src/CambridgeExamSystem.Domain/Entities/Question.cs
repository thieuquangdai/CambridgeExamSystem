namespace CambridgeExamSystem.Domain.Entities;

public class Question
{
    public int QuestionId { get; set; }
    public int ExamSectionId { get; set; }
    public int? QuestionGroupId { get; set; }
    public int QuestionTypeId { get; set; }
    public string? QuestionCode { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? InstructionText { get; set; }
    public int QuestionOrder { get; set; }
    public decimal Score { get; set; } = 1;
    public string? DifficultyCode { get; set; }
    public string? CorrectTextAnswer { get; set; }
    public bool IsRequired { get; set; } = true;
    public bool IsActive { get; set; } = true;

    public Section Section { get; set; } = null!;
    public QuestionGroup? QuestionGroup { get; set; }
    public QuestionType QuestionType { get; set; } = null!;
    public ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();
    public ICollection<QuestionMedia> Media { get; set; } = new List<QuestionMedia>();
    public ICollection<Explanation> Explanations { get; set; } = new List<Explanation>();
}
