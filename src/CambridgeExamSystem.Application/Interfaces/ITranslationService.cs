using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Application.Interfaces;

public interface ITranslationService
{
    Task<TranslationResultDto> TranslateAsync(TranslateRequest request, CancellationToken cancellationToken = default);
}
