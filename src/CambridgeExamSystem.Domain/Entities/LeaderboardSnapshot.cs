using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Domain.Entities;

public class LeaderboardSnapshot
{
    public long LeaderboardSnapshotId { get; set; }
    public int UserId { get; set; }
    public int? LevelId { get; set; }
    public LeaderboardPeriod PeriodType { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
    public int RankNumber { get; set; }
    public int Stars { get; set; }
    public decimal AverageScore { get; set; }
    public int TotalTests { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; } = null!;
    public ExamLevel? Level { get; set; }
}
