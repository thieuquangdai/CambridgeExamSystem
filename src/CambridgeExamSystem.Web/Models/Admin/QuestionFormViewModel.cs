using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Admin;

public class QuestionFormViewModel
{
    public int ExamPaperId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public QuestionEditDto Question { get; set; } = new();
    public IFormFile? ImageFile { get; set; }
    public IFormFile? AudioFile { get; set; }
    public List<QuestionTypeDto> QuestionTypes { get; set; } = [];
}
