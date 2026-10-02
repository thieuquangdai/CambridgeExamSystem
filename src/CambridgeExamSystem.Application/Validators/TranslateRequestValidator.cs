using CambridgeExamSystem.Application.DTOs;
using FluentValidation;

namespace CambridgeExamSystem.Application.Validators;

public sealed class TranslateRequestValidator : AbstractValidator<TranslateRequest>
{
    public TranslateRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().WithMessage("Vui lòng chọn văn bản cần dịch.").MaximumLength(500);
        RuleFor(x => x.SourceLanguage).NotEmpty().Length(2, 5);
        RuleFor(x => x.TargetLanguage).NotEmpty().Length(2, 5);
    }
}
