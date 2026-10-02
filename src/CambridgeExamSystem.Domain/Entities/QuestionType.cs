namespace CambridgeExamSystem.Domain.Entities;

public class QuestionType
{
    public int QuestionTypeId { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool RequiresOptions { get; set; }
    public bool AllowsTextAnswer { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
