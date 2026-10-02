using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Admin;

public class ExamContentViewModel
{
    public ExamContentDto Content { get; set; } = new();
    public SectionEditDto NewSection { get; set; } = new();
}
