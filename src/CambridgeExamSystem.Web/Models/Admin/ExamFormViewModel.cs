using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Admin;

public class ExamFormViewModel
{
    public ExamPaperEditDto Exam { get; set; } = new();
    public List<ExamLevelDto> Levels { get; set; } = [];
}
