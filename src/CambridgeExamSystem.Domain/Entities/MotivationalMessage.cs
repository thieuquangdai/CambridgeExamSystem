namespace CambridgeExamSystem.Domain.Entities;

public class MotivationalMessage
{
    public int MotivationalMessageId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public decimal? MinPercentage { get; set; }
    public decimal? MaxPercentage { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
}
