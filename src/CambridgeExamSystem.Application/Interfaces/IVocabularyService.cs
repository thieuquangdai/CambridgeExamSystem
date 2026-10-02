using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Application.Interfaces;

public interface IVocabularyService
{
    Task<VocabularyDto> SaveAsync(int userId, SaveVocabularyRequest request, CancellationToken cancellationToken = default);
    Task<List<VocabularyDto>> GetAsync(int userId, string? search, bool bookmarkedOnly, CancellationToken cancellationToken = default);
    Task<bool> ToggleBookmarkAsync(int userId, long userVocabularyId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int userId, long userVocabularyId, CancellationToken cancellationToken = default);
}
