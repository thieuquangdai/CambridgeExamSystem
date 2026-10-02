namespace CambridgeExamSystem.Domain.Entities;

public class QuestionGroup
{
    public int QuestionGroupId { get; set; }
    public int ExamSectionId { get; set; }
    public string? GroupTitle { get; set; }
    public string? Instructions { get; set; }
    public int GroupOrder { get; set; }
    public string? PassageText { get; set; }
    public bool IsActive { get; set; } = true;

    public Section Section { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<QuestionMedia> Media { get; set; } = new List<QuestionMedia>();
}
