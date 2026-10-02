namespace CambridgeExamSystem.Domain.Entities;

public class ExamPaper
{
    public int ExamPaperId { get; set; }
    public int LevelId { get; set; }
    public string ExamCode { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalScore { get; set; }
    public decimal? PassPercentage { get; set; }
    public int VersionNo { get; set; } = 1;
    public bool IsPublished { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ExamLevel Level { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
}
