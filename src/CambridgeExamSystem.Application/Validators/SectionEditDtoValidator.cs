using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Domain.Common;
using FluentValidation;

namespace CambridgeExamSystem.Application.Validators;

public sealed class SectionEditDtoValidator : AbstractValidator<SectionEditDto>
{
    public SectionEditDtoValidator()
    {
        RuleFor(x => x.ExamPaperId).GreaterThan(0);
        RuleFor(x => x.SectionCode)
            .NotEmpty()
            .Must(code => SectionCodes.All.Contains(code.Trim().ToUpperInvariant()))
            .WithMessage($"Mã phần thi phải là một trong: {string.Join(", ", SectionCodes.All)}.");
        RuleFor(x => x.SectionName).NotEmpty().WithMessage("Vui lòng nhập tên phần thi.").MaximumLength(200);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).When(x => x.DurationMinutes.HasValue);
    }
}
