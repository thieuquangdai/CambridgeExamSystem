using CambridgeExamSystem.Application.DTOs;
using FluentValidation;

namespace CambridgeExamSystem.Application.Validators;

public sealed class SaveVocabularyRequestValidator : AbstractValidator<SaveVocabularyRequest>
{
    public SaveVocabularyRequestValidator()
    {
        RuleFor(x => x.OriginalText).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.TranslatedText).MaximumLength(1000);
        RuleFor(x => x.Phonetic).MaximumLength(200);
        RuleFor(x => x.AudioUrl).MaximumLength(500);
    }
}
