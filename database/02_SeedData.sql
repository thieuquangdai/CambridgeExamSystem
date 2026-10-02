-- Cambridge Exam System - reference data.
-- The web application seeds the same data automatically at startup (DbInitializer),
-- so this script is only needed when the database is provisioned manually. Safe to re-run.
-- Admin accounts must be created through the application (Seed:AdminEmail / Seed:AdminPassword)
-- so that passwords are hashed by ASP.NET Core Identity.
USE [CambridgeExamDb];
GO

SET NOCOUNT ON;

INSERT INTO [Roles] ([Name], [NormalizedName], [ConcurrencyStamp])
SELECT v.[Name], UPPER(v.[Name]), CONVERT(nvarchar(36), NEWID())
FROM (VALUES (N'Student'), (N'Teacher'), (N'Admin')) AS v([Name])
WHERE NOT EXISTS (SELECT 1 FROM [Roles] r WHERE r.[NormalizedName] = UPPER(v.[Name]));

INSERT INTO [ExamLevels] ([LevelCode], [LevelName], [Description], [DisplayOrder], [IsActive])
SELECT v.[LevelCode], v.[LevelName], v.[Description], v.[DisplayOrder], 1
FROM (VALUES
    ('STARTERS', N'Pre A1 Starters', N'Young Learners – cấp độ khởi đầu.', 1),
    ('MOVERS', N'A1 Movers', N'Young Learners – cấp độ A1.', 2),
    ('FLYERS', N'A2 Flyers', N'Young Learners – cấp độ A2.', 3),
    ('KET', N'A2 Key (KET)', N'Cambridge English A2 Key.', 4),
    ('PET', N'B1 Preliminary (PET)', N'Cambridge English B1 Preliminary.', 5)
) AS v([LevelCode], [LevelName], [Description], [DisplayOrder])
WHERE NOT EXISTS (SELECT 1 FROM [ExamLevels] l WHERE l.[LevelCode] = v.[LevelCode]);

INSERT INTO [QuestionTypes] ([TypeCode], [TypeName], [RequiresOptions], [AllowsTextAnswer], [IsActive])
SELECT v.[TypeCode], v.[TypeName], v.[RequiresOptions], v.[AllowsTextAnswer], 1
FROM (VALUES
    ('MULTIPLE_CHOICE', N'Trắc nghiệm một đáp án', 1, 0),
    ('MULTIPLE_SELECT', N'Trắc nghiệm nhiều đáp án', 1, 0),
    ('TRUE_FALSE', N'Đúng / Sai', 1, 0),
    ('FILL_BLANK', N'Điền vào chỗ trống', 0, 1),
    ('SHORT_ANSWER', N'Trả lời ngắn', 0, 1),
    ('MATCHING', N'Nối', 1, 0),
    ('ORDERING', N'Sắp xếp thứ tự', 1, 0)
) AS v([TypeCode], [TypeName], [RequiresOptions], [AllowsTextAnswer])
WHERE NOT EXISTS (SELECT 1 FROM [QuestionTypes] t WHERE t.[TypeCode] = v.[TypeCode]);

INSERT INTO [Achievements] ([AchievementCode], [AchievementName], [Description], [RequiredTests], [RequiredScore], [RewardStars], [BadgeColor], [IsActive])
SELECT v.[AchievementCode], v.[AchievementName], v.[Description], v.[RequiredTests], v.[RequiredScore], v.[RewardStars], v.[BadgeColor], 1
FROM (VALUES
    ('FIRST_TEST', N'Bước đầu tiên', N'Hoàn thành bài thi đầu tiên.', 1, NULL, 5, 'primary'),
    ('FIVE_TESTS', N'Chăm chỉ', N'Hoàn thành 5 bài thi.', 5, NULL, 10, 'info'),
    ('TEN_TESTS', N'Bền bỉ', N'Hoàn thành 10 bài thi.', 10, NULL, 20, 'success'),
    ('HIGH_SCORER', N'Điểm cao', N'Đạt từ 90% trở lên trong một bài thi.', NULL, 90, 10, 'warning'),
    ('PERFECT_SCORE', N'Hoàn hảo', N'Đạt 100% trong một bài thi.', NULL, 100, 20, 'danger')
) AS v([AchievementCode], [AchievementName], [Description], [RequiredTests], [RequiredScore], [RewardStars], [BadgeColor])
WHERE NOT EXISTS (SELECT 1 FROM [Achievements] a WHERE a.[AchievementCode] = v.[AchievementCode]);

IF NOT EXISTS (SELECT 1 FROM [MotivationalMessages])
BEGIN
    INSERT INTO [MotivationalMessages] ([MessageType], [MinPercentage], [MaxPercentage], [MessageText], [IsActive])
    VALUES
        ('RESULT', 90, 100, N'Xuất sắc! Bạn đã nắm rất vững kiến thức.', 1),
        ('RESULT', 70, 89.99, N'Rất tốt! Chỉ cần thêm một chút nữa là đạt điểm tối đa.', 1),
        ('RESULT', 50, 69.99, N'Khá lắm! Hãy xem lại phần giải thích để tiến bộ hơn.', 1),
        ('RESULT', 0, 49.99, N'Đừng nản lòng! Mỗi lần luyện tập là một bước tiến.', 1);
END;
GO
