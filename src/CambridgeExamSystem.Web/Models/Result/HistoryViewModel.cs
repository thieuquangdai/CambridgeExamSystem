using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Web.Models.Result;

public class HistoryViewModel
{
    public List<AttemptHistoryItemDto> Attempts { get; set; } = [];
    public List<ActiveAttemptDto> ActiveAttempts { get; set; } = [];
}
