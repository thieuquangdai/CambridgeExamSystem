using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Interfaces.Repositories;

public interface ILeaderboardRepository
{
    Task<List<LeaderboardEntryDto>> GetEntriesAsync(DateTime? fromUtc, DateTime? toUtc, int? levelId, CancellationToken cancellationToken = default);
    void AddSnapshots(IEnumerable<LeaderboardSnapshot> snapshots);
}
