namespace CambridgeExamSystem.Domain.Entities;

public class Section
{
    public int ExamSectionId { get; set; }
    public int ExamPaperId { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string? SectionDescription { get; set; }
    public int SectionOrder { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal MaxScore { get; set; }

    public ExamPaper ExamPaper { get; set; } = null!;
    public ICollection<QuestionGroup> QuestionGroups { get; set; } = new List<QuestionGroup>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
