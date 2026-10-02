using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Application.Validators;

namespace CambridgeExamSystem.Tests.Application;

public class ValidatorTests
{
    [Fact]
    public void Exam_paper_requires_code_name_level_and_duration()
    {
        var result = new ExamPaperEditDtoValidator().Validate(new ExamPaperEditDto { ExamCode = "bad code!", DurationMinutes = 0 });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ExamPaperEditDto.ExamCode));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ExamPaperEditDto.ExamName));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ExamPaperEditDto.LevelId));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ExamPaperEditDto.DurationMinutes));
    }

    [Fact]
    public void Valid_exam_paper_passes()
    {
        var dto = new ExamPaperEditDto { ExamCode = "KET-01", ExamName = "KET 01", LevelId = 4, DurationMinutes = 60, PassPercentage = 60 };

        Assert.True(new ExamPaperEditDtoValidator().Validate(dto).IsValid);
    }

    [Fact]
    public void Save_answer_rejects_negative_time_and_invalid_attempt()
    {
        var result = new SaveAnswerRequestValidator().Validate(new SaveAnswerRequest { ExamAttemptId = 0, TimeSpentSeconds = -1 });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SaveAnswerRequest.ExamAttemptId));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(SaveAnswerRequest.TimeSpentSeconds));
    }

    [Fact]
    public void Question_requires_text_and_type()
    {
        var result = new QuestionEditDtoValidator().Validate(new QuestionEditDto { ExamSectionId = 1, DifficultyCode = "Impossible" });

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(QuestionEditDto.QuestionText));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(QuestionEditDto.QuestionTypeId));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(QuestionEditDto.DifficultyCode));
    }
}
