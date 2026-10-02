using CambridgeExamSystem.Application.Services;
using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Tests.Application;

public class GradingServiceTests
{
    private readonly GradingService _grading = new();

    private static Question CreateQuestion(string typeCode, decimal score = 2, string? correctText = null, params AnswerOption[] options)
    {
        var question = new Question
        {
            QuestionId = 1,
            QuestionText = "Q",
            Score = score,
            CorrectTextAnswer = correctText,
            QuestionType = new QuestionType { TypeCode = typeCode }
        };
        foreach (var option in options)
        {
            option.QuestionId = question.QuestionId;
            question.Options.Add(option);
        }

        return question;
    }

    private static AnswerOption Option(int id, int order, bool correct = false, string? key = null) =>
        new() { QuestionOptionId = id, OptionOrder = order, OptionText = $"Option {id}", IsCorrect = correct, MatchingKey = key };

    [Fact]
    public void Unanswered_question_scores_zero()
    {
        var question = CreateQuestion(QuestionTypeCodes.MultipleChoice, options: [Option(1, 1, true), Option(2, 2)]);

        var outcome = _grading.Grade(question, new TestAnswer());

        Assert.Equal(new GradeOutcome(false, false, 0), outcome);
        Assert.Equal(new GradeOutcome(false, false, 0), _grading.Grade(question, null));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(99, false)]
    public void Multiple_choice_is_correct_only_for_the_correct_option(int selected, bool expected)
    {
        var question = CreateQuestion(QuestionTypeCodes.MultipleChoice, options: [Option(1, 1, true), Option(2, 2)]);

        var outcome = _grading.Grade(question, new TestAnswer { SelectedOptionId = selected });

        Assert.True(outcome.IsAnswered);
        Assert.Equal(expected, outcome.IsCorrect);
        Assert.Equal(expected ? 2 : 0, outcome.ScoreEarned);
    }

    [Theory]
    [InlineData("""{"selectedOptionIds":[1,3]}""", true)]
    [InlineData("""{"selectedOptionIds":[3,1]}""", true)]
    [InlineData("""{"selectedOptionIds":[1]}""", false)]
    [InlineData("""{"selectedOptionIds":[1,2,3]}""", false)]
    public void Multiple_select_requires_the_exact_set(string json, bool expected)
    {
        var question = CreateQuestion(QuestionTypeCodes.MultipleSelect, options: [Option(1, 1, true), Option(2, 2), Option(3, 3, true)]);

        Assert.Equal(expected, _grading.Grade(question, new TestAnswer { AnswerJson = json }).IsCorrect);
    }

    [Theory]
    [InlineData("  Went   to SCHOOL. ", true)]
    [InlineData("go to school", true)]
    [InlineData("went school", false)]
    public void Text_answers_ignore_case_whitespace_and_accept_alternatives(string answer, bool expected)
    {
        var question = CreateQuestion(QuestionTypeCodes.FillBlank, correctText: "went to school|go to school");

        Assert.Equal(expected, _grading.Grade(question, new TestAnswer { TextAnswer = answer }).IsCorrect);
    }

    [Fact]
    public void Text_question_without_answer_key_is_never_correct()
    {
        var question = CreateQuestion(QuestionTypeCodes.ShortAnswer);

        var outcome = _grading.Grade(question, new TestAnswer { TextAnswer = "anything" });

        Assert.True(outcome.IsAnswered);
        Assert.False(outcome.IsCorrect);
    }

    [Theory]
    [InlineData("""{"matches":{"1":"b","2":"A"}}""", true)]
    [InlineData("""{"matches":{"1":"A","2":"B"}}""", false)]
    [InlineData("""{"matches":{"1":"B"}}""", false)]
    public void Matching_requires_every_pair(string json, bool expected)
    {
        var question = CreateQuestion(QuestionTypeCodes.Matching, options: [Option(1, 1, key: "B"), Option(2, 2, key: "A")]);

        Assert.Equal(expected, _grading.Grade(question, new TestAnswer { AnswerJson = json }).IsCorrect);
    }

    [Theory]
    [InlineData("""{"orderedOptionIds":[10,11,12]}""", true)]
    [InlineData("""{"orderedOptionIds":[11,10,12]}""", false)]
    public void Ordering_follows_option_order(string json, bool expected)
    {
        var question = CreateQuestion(QuestionTypeCodes.Ordering, options: [Option(12, 3), Option(10, 1), Option(11, 2)]);

        Assert.Equal(expected, _grading.Grade(question, new TestAnswer { AnswerJson = json }).IsCorrect);
    }

    [Fact]
    public void Malformed_json_is_treated_as_unanswered()
    {
        var question = CreateQuestion(QuestionTypeCodes.MultipleSelect, options: [Option(1, 1, true)]);

        Assert.False(_grading.Grade(question, new TestAnswer { AnswerJson = "{not json" }).IsAnswered);
    }

    [Fact]
    public void Describes_correct_answers_for_review()
    {
        var matching = CreateQuestion(QuestionTypeCodes.Matching, options: [Option(1, 1, key: "B"), Option(2, 2, key: "A")]);
        var text = CreateQuestion(QuestionTypeCodes.FillBlank, correctText: "went|go");

        Assert.Equal("Option 1 → B; Option 2 → A", _grading.DescribeCorrectAnswer(matching));
        Assert.Equal("went", _grading.DescribeCorrectAnswer(text));
    }
}
