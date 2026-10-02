using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Common;

public class HomeViewModel
{
    public List<ExamLevelDto> Levels { get; set; } = [];
    public List<ExamPaperSummaryDto> LatestExams { get; set; } = [];
    public List<ActiveAttemptDto> ActiveAttempts { get; set; } = [];
}
