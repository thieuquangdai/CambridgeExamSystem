using AutoMapper;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Interfaces;
using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Services;

public sealed class VocabularyService(
    IVocabularyRepository vocabularyRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    TimeProvider timeProvider) : IVocabularyService
{
    public async Task<VocabularyDto> SaveAsync(int userId, SaveVocabularyRequest request, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var text = request.OriginalText.Trim();
        var vocabulary = await vocabularyRepository.FindByTextAsync(userId, text, cancellationToken);

        if (vocabulary is null)
        {
            vocabulary = new UserVocabulary { UserId = userId, OriginalText = text, CreatedAtUtc = now, Frequency = 0 };
            vocabularyRepository.Add(vocabulary);
        }

        vocabulary.Frequency++;
        vocabulary.LastViewedAtUtc = now;
        vocabulary.TranslatedText = request.TranslatedText ?? vocabulary.TranslatedText;
        vocabulary.DefinitionText = request.DefinitionText ?? vocabulary.DefinitionText;
        vocabulary.ExampleText = request.ExampleText ?? vocabulary.ExampleText;
        vocabulary.Phonetic = request.Phonetic ?? vocabulary.Phonetic;
        vocabulary.AudioUrl = request.AudioUrl ?? vocabulary.AudioUrl;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<VocabularyDto>(vocabulary);
    }

    public async Task<List<VocabularyDto>> GetAsync(int userId, string? search, bool bookmarkedOnly, CancellationToken cancellationToken = default)
    {
        var items = await vocabularyRepository.ListAsync(userId, search?.Trim(), bookmarkedOnly, cancellationToken);
        return items.Select(mapper.Map<VocabularyDto>).ToList();
    }

    public async Task<bool> ToggleBookmarkAsync(int userId, long userVocabularyId, CancellationToken cancellationToken = default)
    {
        var vocabulary = await vocabularyRepository.GetAsync(userId, userVocabularyId, cancellationToken);
        if (vocabulary is null)
        {
            return false;
        }

        vocabulary.IsBookmarked = !vocabulary.IsBookmarked;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return vocabulary.IsBookmarked;
    }

    public async Task<bool> DeleteAsync(int userId, long userVocabularyId, CancellationToken cancellationToken = default)
    {
        var vocabulary = await vocabularyRepository.GetAsync(userId, userVocabularyId, cancellationToken);
        if (vocabulary is null)
        {
            return false;
        }

        vocabularyRepository.Remove(vocabulary);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
