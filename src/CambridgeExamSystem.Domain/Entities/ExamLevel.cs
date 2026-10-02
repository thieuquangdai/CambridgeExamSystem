namespace CambridgeExamSystem.Domain.Entities;

public class ExamLevel
{
    public int LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ExamPaper> ExamPapers { get; set; } = new List<ExamPaper>();
}
