using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Data;

public static class SampleExamSeeder
{
    public const string SampleExamCode = "KET-SAMPLE-01";

    public static async Task SeedAsync(CambridgeDbContext context, int? createdByUserId, DateTime now, CancellationToken cancellationToken = default)
    {
        if (await context.ExamPapers.AnyAsync(p => p.ExamCode == SampleExamCode, cancellationToken))
        {
            return;
        }

        var level = await context.ExamLevels.SingleAsync(l => l.LevelCode == "KET", cancellationToken);
        var types = await context.QuestionTypes.ToDictionaryAsync(t => t.TypeCode, cancellationToken);

        var paper = new ExamPaper
        {
            Level = level,
            ExamCode = SampleExamCode,
            ExamName = "A2 Key – Đề luyện tập số 1",
            Description = "Đề mẫu gồm Reading và Writing, minh hoạ đủ 7 dạng câu hỏi.",
            DurationMinutes = 30,
            PassPercentage = 70,
            IsPublished = true,
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = now
        };

        var reading = new Section { SectionCode = SectionCodes.Reading, SectionName = "Reading", SectionOrder = 1, SectionDescription = "Đọc hiểu" };
        var group = new QuestionGroup
        {
            GroupOrder = 1,
            GroupTitle = "A day at the zoo",
            Instructions = "Read the text and answer questions 1–4.",
            PassageText = "Last Saturday, Anna went to the city zoo with her brother Tom. They arrived at nine o'clock and saw the lions first. "
                + "Tom liked the monkeys best because they were very funny. At lunchtime they ate sandwiches near the lake. "
                + "In the afternoon it started to rain, so they visited the reptile house. They went home by bus at four o'clock."
        };
        reading.QuestionGroups.Add(group);

        var order = 1;
        reading.Questions.Add(Choice(types[QuestionTypeCodes.MultipleChoice], group, order++,
            "Which animals did Anna and Tom see first?",
            [("The monkeys", false), ("The lions", true), ("The snakes", false)],
            "Đoạn văn viết: \"They arrived at nine o'clock and saw the lions first.\""));
        reading.Questions.Add(Choice(types[QuestionTypeCodes.TrueFalse], group, order++,
            "Anna and Tom went home by train.",
            [("True", false), ("False", true)],
            "Họ về nhà bằng xe buýt: \"They went home by bus\"."));
        reading.Questions.Add(Text(types[QuestionTypeCodes.FillBlank], group, order++,
            "At lunchtime they ate ________ near the lake.", "sandwiches|sandwich",
            "\"At lunchtime they ate sandwiches near the lake.\""));
        reading.Questions.Add(Choice(types[QuestionTypeCodes.MultipleSelect], group, order++,
            "Which TWO places did they visit? (Chọn 2 đáp án)",
            [("The lake", true), ("The museum", false), ("The reptile house", true), ("The cinema", false)],
            "Họ ăn trưa cạnh hồ và vào nhà bò sát khi trời mưa."));

        var matching = Choice(types[QuestionTypeCodes.Matching], null, order++,
            "Match each word with its meaning.",
            [("big", false), ("happy", false), ("cold", false)],
            "big = large, happy = glad, cold = not warm.");
        var keys = new[] { "large", "glad", "not warm" };
        foreach (var (option, key) in matching.Options.Zip(keys))
        {
            option.MatchingKey = key;
        }

        reading.Questions.Add(matching);
        reading.Questions.Add(Choice(types[QuestionTypeCodes.Ordering], null, order++,
            "Put the words in the correct order to make a sentence.",
            [("She", false), ("goes", false), ("to school", false), ("every day", false)],
            "Câu đúng: \"She goes to school every day.\""));

        var writing = new Section { SectionCode = SectionCodes.Writing, SectionName = "Writing", SectionOrder = 2, SectionDescription = "Viết" };
        writing.Questions.Add(Text(types[QuestionTypeCodes.ShortAnswer], null, 1,
            "Write the past tense of the verb \"go\".", "went",
            "\"go\" là động từ bất quy tắc: go – went – gone."));
        writing.Questions.Add(Text(types[QuestionTypeCodes.FillBlank], null, 2,
            "I have two ________ (child).", "children",
            "Danh từ số nhiều bất quy tắc: child → children."));

        foreach (var section in new[] { reading, writing })
        {
            section.MaxScore = section.Questions.Sum(q => q.Score);
            paper.Sections.Add(section);
        }

        paper.TotalScore = paper.Sections.Sum(s => s.MaxScore);
        context.ExamPapers.Add(paper);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Question Choice(QuestionType type, QuestionGroup? group, int order, string text, (string Text, bool IsCorrect)[] options, string explanation)
    {
        var question = NewQuestion(type, group, order, text, explanation);
        for (var i = 0; i < options.Length; i++)
        {
            question.Options.Add(new AnswerOption
            {
                OptionCode = ((char)('A' + i)).ToString(),
                OptionText = options[i].Text,
                OptionOrder = i + 1,
                IsCorrect = options[i].IsCorrect
            });
        }

        return question;
    }

    private static Question Text(QuestionType type, QuestionGroup? group, int order, string text, string correctAnswer, string explanation)
    {
        var question = NewQuestion(type, group, order, text, explanation);
        question.CorrectTextAnswer = correctAnswer;
        return question;
    }

    private static Question NewQuestion(QuestionType type, QuestionGroup? group, int order, string text, string explanation)
    {
        var question = new Question
        {
            QuestionType = type,
            QuestionGroup = group,
            QuestionCode = $"Q{order}",
            QuestionText = text,
            QuestionOrder = order,
            Score = 1,
            DifficultyCode = "Easy"
        };
        question.Explanations.Add(new Explanation { ExplanationText = explanation });
        return question;
    }
}
