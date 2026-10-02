using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Repositories;

public sealed class VocabularyRepository(CambridgeDbContext context) : IVocabularyRepository
{
    public Task<UserVocabulary?> FindByTextAsync(int userId, string originalText, CancellationToken cancellationToken = default) =>
        context.UserVocabulary.FirstOrDefaultAsync(v => v.UserId == userId && v.OriginalText == originalText, cancellationToken);

    public Task<UserVocabulary?> GetAsync(int userId, long userVocabularyId, CancellationToken cancellationToken = default) =>
        context.UserVocabulary.FirstOrDefaultAsync(v => v.UserId == userId && v.UserVocabularyId == userVocabularyId, cancellationToken);

    public Task<List<UserVocabulary>> ListAsync(int userId, string? search, bool bookmarkedOnly, CancellationToken cancellationToken = default)
    {
        var query = context.UserVocabulary.AsNoTracking().Where(v => v.UserId == userId);

        if (bookmarkedOnly)
        {
            query = query.Where(v => v.IsBookmarked);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v => v.OriginalText.Contains(search) || (v.TranslatedText != null && v.TranslatedText.Contains(search)));
        }

        return query.OrderByDescending(v => v.LastViewedAtUtc ?? v.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public void Add(UserVocabulary vocabulary) => context.UserVocabulary.Add(vocabulary);

    public void Remove(UserVocabulary vocabulary) => context.UserVocabulary.Remove(vocabulary);
}
