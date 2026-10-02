using CambridgeExamSystem.Domain.Common;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Infrastructure.Data;

public static class ReferenceData
{
    public static IEnumerable<ExamLevel> Levels() =>
    [
        new() { LevelCode = "STARTERS", LevelName = "Pre A1 Starters", Description = "Young Learners – cấp độ khởi đầu.", DisplayOrder = 1 },
        new() { LevelCode = "MOVERS", LevelName = "A1 Movers", Description = "Young Learners – cấp độ A1.", DisplayOrder = 2 },
        new() { LevelCode = "FLYERS", LevelName = "A2 Flyers", Description = "Young Learners – cấp độ A2.", DisplayOrder = 3 },
        new() { LevelCode = "KET", LevelName = "A2 Key (KET)", Description = "Cambridge English A2 Key.", DisplayOrder = 4 },
        new() { LevelCode = "PET", LevelName = "B1 Preliminary (PET)", Description = "Cambridge English B1 Preliminary.", DisplayOrder = 5 }
    ];

    public static IEnumerable<QuestionType> QuestionTypes() =>
    [
        new() { TypeCode = QuestionTypeCodes.MultipleChoice, TypeName = "Trắc nghiệm một đáp án", RequiresOptions = true },
        new() { TypeCode = QuestionTypeCodes.MultipleSelect, TypeName = "Trắc nghiệm nhiều đáp án", RequiresOptions = true },
        new() { TypeCode = QuestionTypeCodes.TrueFalse, TypeName = "Đúng / Sai", RequiresOptions = true },
        new() { TypeCode = QuestionTypeCodes.FillBlank, TypeName = "Điền vào chỗ trống", AllowsTextAnswer = true },
        new() { TypeCode = QuestionTypeCodes.ShortAnswer, TypeName = "Trả lời ngắn", AllowsTextAnswer = true },
        new() { TypeCode = QuestionTypeCodes.Matching, TypeName = "Nối", RequiresOptions = true },
        new() { TypeCode = QuestionTypeCodes.Ordering, TypeName = "Sắp xếp thứ tự", RequiresOptions = true }
    ];

    public static IEnumerable<Achievement> Achievements() =>
    [
        new() { AchievementCode = "FIRST_TEST", AchievementName = "Bước đầu tiên", Description = "Hoàn thành bài thi đầu tiên.", RequiredTests = 1, RewardStars = 5, BadgeColor = "primary" },
        new() { AchievementCode = "FIVE_TESTS", AchievementName = "Chăm chỉ", Description = "Hoàn thành 5 bài thi.", RequiredTests = 5, RewardStars = 10, BadgeColor = "info" },
        new() { AchievementCode = "TEN_TESTS", AchievementName = "Bền bỉ", Description = "Hoàn thành 10 bài thi.", RequiredTests = 10, RewardStars = 20, BadgeColor = "success" },
        new() { AchievementCode = "HIGH_SCORER", AchievementName = "Điểm cao", Description = "Đạt từ 90% trở lên trong một bài thi.", RequiredScore = 90, RewardStars = 10, BadgeColor = "warning" },
        new() { AchievementCode = "PERFECT_SCORE", AchievementName = "Hoàn hảo", Description = "Đạt 100% trong một bài thi.", RequiredScore = 100, RewardStars = 20, BadgeColor = "danger" }
    ];

    public static IEnumerable<MotivationalMessage> MotivationalMessages(DateTime now) =>
    [
        new() { MessageType = "RESULT", MinPercentage = 90, MaxPercentage = 100, MessageText = "Xuất sắc! Bạn đã nắm rất vững kiến thức.", CreatedAtUtc = now },
        new() { MessageType = "RESULT", MinPercentage = 70, MaxPercentage = 89.99m, MessageText = "Rất tốt! Chỉ cần thêm một chút nữa là đạt điểm tối đa.", CreatedAtUtc = now },
        new() { MessageType = "RESULT", MinPercentage = 50, MaxPercentage = 69.99m, MessageText = "Khá lắm! Hãy xem lại phần giải thích để tiến bộ hơn.", CreatedAtUtc = now },
        new() { MessageType = "RESULT", MinPercentage = 0, MaxPercentage = 49.99m, MessageText = "Đừng nản lòng! Mỗi lần luyện tập là một bước tiến.", CreatedAtUtc = now }
    ];
}
