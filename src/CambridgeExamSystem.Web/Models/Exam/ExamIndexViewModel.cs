using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Exam;

public class ExamIndexViewModel
{
    public List<ExamLevelDto> Levels { get; set; } = [];
    public List<ExamPaperSummaryDto> Exams { get; set; } = [];
    public string? SelectedLevel { get; set; }
}
