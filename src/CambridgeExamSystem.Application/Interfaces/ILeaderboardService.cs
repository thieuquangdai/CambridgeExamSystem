using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Domain.Enums;

namespace CambridgeExamSystem.Application.Interfaces;

public interface ILeaderboardService
{
    Task<LeaderboardDto> GetLeaderboardAsync(LeaderboardPeriod period, int? levelId, int? currentUserId, int top = 50, CancellationToken cancellationToken = default);
    Task<int> CreateSnapshotAsync(LeaderboardPeriod period, int? levelId, CancellationToken cancellationToken = default);
}
