using CambridgeExamSystem.Application.DTOs;
using FluentValidation;

namespace CambridgeExamSystem.Application.Validators;

public sealed class ExamPaperEditDtoValidator : AbstractValidator<ExamPaperEditDto>
{
    public ExamPaperEditDtoValidator()
    {
        RuleFor(x => x.LevelId).GreaterThan(0).WithMessage("Vui lòng chọn cấp độ.");
        RuleFor(x => x.ExamCode)
            .NotEmpty().WithMessage("Vui lòng nhập mã đề.")
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Mã đề chỉ gồm chữ, số, '-' và '_'.");
        RuleFor(x => x.ExamName).NotEmpty().WithMessage("Vui lòng nhập tên đề.").MaximumLength(250);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(1, 600).WithMessage("Thời gian làm bài từ 1 đến 600 phút.");
        RuleFor(x => x.PassPercentage).InclusiveBetween(0, 100).When(x => x.PassPercentage.HasValue);
    }
}
