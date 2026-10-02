using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Interfaces.Repositories;

public interface IVocabularyRepository
{
    Task<UserVocabulary?> FindByTextAsync(int userId, string originalText, CancellationToken cancellationToken = default);
    Task<UserVocabulary?> GetAsync(int userId, long userVocabularyId, CancellationToken cancellationToken = default);
    Task<List<UserVocabulary>> ListAsync(int userId, string? search, bool bookmarkedOnly, CancellationToken cancellationToken = default);
    void Add(UserVocabulary vocabulary);
    void Remove(UserVocabulary vocabulary);
}
