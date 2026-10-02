using CambridgeExamSystem.Application.DTOs;
using FluentValidation;

namespace CambridgeExamSystem.Application.Validators;

public sealed class QuestionEditDtoValidator : AbstractValidator<QuestionEditDto>
{
    private static readonly string[] Difficulties = ["Easy", "Medium", "Hard"];

    public QuestionEditDtoValidator()
    {
        RuleFor(x => x.ExamSectionId).GreaterThan(0);
        RuleFor(x => x.QuestionTypeId).GreaterThan(0).WithMessage("Vui lòng chọn dạng câu hỏi.");
        RuleFor(x => x.QuestionText).NotEmpty().WithMessage("Vui lòng nhập nội dung câu hỏi.");
        RuleFor(x => x.QuestionCode).MaximumLength(50);
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
        RuleFor(x => x.DifficultyCode)
            .Must(d => d is null || Difficulties.Contains(d))
            .WithMessage("Độ khó phải là Easy, Medium hoặc Hard.");
        RuleFor(x => x.CorrectTextAnswer).MaximumLength(2000);
        RuleFor(x => x.ImageUrl).MaximumLength(1000);
        RuleFor(x => x.AudioUrl).MaximumLength(1000);
        RuleForEach(x => x.Options).ChildRules(option =>
        {
            option.RuleFor(o => o.MatchingKey).MaximumLength(100);
        });
    }
}
