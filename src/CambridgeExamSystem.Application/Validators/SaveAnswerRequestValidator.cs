using CambridgeExamSystem.Application.DTOs;
using FluentValidation;

namespace CambridgeExamSystem.Application.Validators;

public sealed class SaveAnswerRequestValidator : AbstractValidator<SaveAnswerRequest>
{
    public SaveAnswerRequestValidator()
    {
        RuleFor(x => x.ExamAttemptId).GreaterThan(0);
        RuleFor(x => x.QuestionId).GreaterThan(0).When(x => x.QuestionId.HasValue);
        RuleFor(x => x.TimeSpentSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentQuestionIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TextAnswer).MaximumLength(4000);
        RuleFor(x => x.AnswerJson).MaximumLength(8000);
    }
}
