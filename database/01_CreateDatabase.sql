-- Cambridge Exam System - database schema (SQL Server 2019+).
-- Generated from the EF Core migrations with:
--   dotnet ef migrations script --idempotent -p src/CambridgeExamSystem.Infrastructure -s src/CambridgeExamSystem.Infrastructure
-- Regenerate this file after adding a migration. The script is idempotent and safe to re-run.
IF DB_ID(N'CambridgeExamDb') IS NULL
    CREATE DATABASE [CambridgeExamDb];
GO

USE [CambridgeExamDb];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [Achievements] (
        [AchievementId] int NOT NULL IDENTITY,
        [AchievementCode] varchar(50) NOT NULL,
        [AchievementName] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NOT NULL,
        [IconUrl] nvarchar(500) NULL,
        [BadgeColor] varchar(30) NULL,
        [RequiredTests] int NULL,
        [RequiredScore] decimal(10,2) NULL,
        [RewardStars] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Achievements] PRIMARY KEY ([AchievementId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [ExamLevels] (
        [LevelId] int NOT NULL IDENTITY,
        [LevelCode] varchar(30) NOT NULL,
        [LevelName] nvarchar(100) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [DisplayOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_ExamLevels] PRIMARY KEY ([LevelId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [MotivationalMessages] (
        [MotivationalMessageId] int NOT NULL IDENTITY,
        [MessageType] varchar(30) NOT NULL,
        [MessageText] nvarchar(1000) NOT NULL,
        [MinPercentage] decimal(5,2) NULL,
        [MaxPercentage] decimal(5,2) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_MotivationalMessages] PRIMARY KEY ([MotivationalMessageId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [QuestionTypes] (
        [QuestionTypeId] int NOT NULL IDENTITY,
        [TypeCode] varchar(50) NOT NULL,
        [TypeName] nvarchar(150) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [RequiresOptions] bit NOT NULL,
        [AllowsTextAnswer] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_QuestionTypes] PRIMARY KEY ([QuestionTypeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [FullName] nvarchar(200) NOT NULL,
        [DateOfBirth] date NULL,
        [AvatarUrl] nvarchar(500) NULL,
        [CurrentLevelCode] varchar(30) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [UpdatedAtUtc] datetime2(0) NULL,
        [LastLoginAtUtc] datetime2(0) NULL,
        [UserName] nvarchar(100) NULL,
        [NormalizedUserName] nvarchar(100) NULL,
        [Email] nvarchar(255) NULL,
        [NormalizedEmail] nvarchar(255) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(500) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [ExamPapers] (
        [ExamPaperId] int NOT NULL IDENTITY,
        [LevelId] int NOT NULL,
        [ExamCode] varchar(50) NOT NULL,
        [ExamName] nvarchar(250) NOT NULL,
        [Description] nvarchar(max) NULL,
        [DurationMinutes] int NOT NULL,
        [TotalScore] decimal(10,2) NOT NULL,
        [PassPercentage] decimal(5,2) NULL,
        [VersionNo] int NOT NULL,
        [IsPublished] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedByUserId] int NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [UpdatedAtUtc] datetime2(0) NULL,
        CONSTRAINT [PK_ExamPapers] PRIMARY KEY ([ExamPaperId]),
        CONSTRAINT [CK_ExamPapers_Duration] CHECK ([DurationMinutes] > 0),
        CONSTRAINT [CK_ExamPapers_TotalScore] CHECK ([TotalScore] >= 0),
        CONSTRAINT [FK_ExamPapers_ExamLevels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [ExamLevels] ([LevelId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamPapers_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [LeaderboardSnapshots] (
        [LeaderboardSnapshotId] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [LevelId] int NULL,
        [PeriodType] varchar(20) NOT NULL,
        [PeriodStartDate] date NOT NULL,
        [PeriodEndDate] date NOT NULL,
        [RankNumber] int NOT NULL,
        [Stars] int NOT NULL,
        [AverageScore] decimal(5,2) NOT NULL,
        [TotalTests] int NOT NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_LeaderboardSnapshots] PRIMARY KEY ([LeaderboardSnapshotId]),
        CONSTRAINT [CK_LeaderboardSnapshots_PeriodType] CHECK ([PeriodType] IN ('Weekly', 'Monthly', 'AllTime')),
        CONSTRAINT [FK_LeaderboardSnapshots_ExamLevels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [ExamLevels] ([LevelId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LeaderboardSnapshots_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserAchievements] (
        [UserAchievementId] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [AchievementId] int NOT NULL,
        [EarnedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_UserAchievements] PRIMARY KEY ([UserAchievementId]),
        CONSTRAINT [FK_UserAchievements_Achievements_AchievementId] FOREIGN KEY ([AchievementId]) REFERENCES [Achievements] ([AchievementId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserAchievements_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserStatistics] (
        [UserId] int NOT NULL,
        [TotalTests] int NOT NULL,
        [PassedTests] int NOT NULL,
        [TotalStars] int NOT NULL,
        [AverageScore] decimal(5,2) NOT NULL,
        [BestPercentage] decimal(5,2) NOT NULL,
        [CurrentRankCode] varchar(30) NOT NULL DEFAULT 'Bronze',
        [CurrentStreakDays] int NOT NULL,
        [LongestStreakDays] int NOT NULL,
        [LastActivityDate] date NULL,
        [TotalLearningSeconds] bigint NOT NULL,
        [UpdatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_UserStatistics] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_UserStatistics_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [UserVocabulary] (
        [UserVocabularyId] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [OriginalText] nvarchar(1000) NOT NULL,
        [TranslatedText] nvarchar(1000) NULL,
        [DefinitionText] nvarchar(max) NULL,
        [ExampleText] nvarchar(max) NULL,
        [Phonetic] nvarchar(200) NULL,
        [AudioUrl] nvarchar(500) NULL,
        [Frequency] int NOT NULL,
        [IsBookmarked] bit NOT NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [LastViewedAtUtc] datetime2(0) NULL,
        CONSTRAINT [PK_UserVocabulary] PRIMARY KEY ([UserVocabularyId]),
        CONSTRAINT [FK_UserVocabulary_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [ExamSections] (
        [ExamSectionId] int NOT NULL IDENTITY,
        [ExamPaperId] int NOT NULL,
        [SectionCode] varchar(30) NOT NULL,
        [SectionName] nvarchar(200) NOT NULL,
        [SectionDescription] nvarchar(max) NULL,
        [SectionOrder] int NOT NULL,
        [DurationMinutes] int NULL,
        [MaxScore] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_ExamSections] PRIMARY KEY ([ExamSectionId]),
        CONSTRAINT [FK_ExamSections_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([ExamPaperId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [QuestionGroups] (
        [QuestionGroupId] int NOT NULL IDENTITY,
        [ExamSectionId] int NOT NULL,
        [GroupTitle] nvarchar(250) NULL,
        [Instructions] nvarchar(max) NULL,
        [GroupOrder] int NOT NULL,
        [PassageText] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_QuestionGroups] PRIMARY KEY ([QuestionGroupId]),
        CONSTRAINT [FK_QuestionGroups_ExamSections_ExamSectionId] FOREIGN KEY ([ExamSectionId]) REFERENCES [ExamSections] ([ExamSectionId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [Questions] (
        [QuestionId] int NOT NULL IDENTITY,
        [ExamSectionId] int NOT NULL,
        [QuestionGroupId] int NULL,
        [QuestionTypeId] int NOT NULL,
        [QuestionCode] varchar(50) NULL,
        [QuestionText] nvarchar(max) NOT NULL,
        [InstructionText] nvarchar(max) NULL,
        [QuestionOrder] int NOT NULL,
        [Score] decimal(10,2) NOT NULL,
        [DifficultyCode] varchar(30) NULL,
        [CorrectTextAnswer] nvarchar(2000) NULL,
        [IsRequired] bit NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([QuestionId]),
        CONSTRAINT [CK_Questions_Score] CHECK ([Score] >= 0),
        CONSTRAINT [FK_Questions_ExamSections_ExamSectionId] FOREIGN KEY ([ExamSectionId]) REFERENCES [ExamSections] ([ExamSectionId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Questions_QuestionGroups_QuestionGroupId] FOREIGN KEY ([QuestionGroupId]) REFERENCES [QuestionGroups] ([QuestionGroupId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Questions_QuestionTypes_QuestionTypeId] FOREIGN KEY ([QuestionTypeId]) REFERENCES [QuestionTypes] ([QuestionTypeId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [ExamAttempts] (
        [ExamAttemptId] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ExamPaperId] int NOT NULL,
        [AttemptNumber] int NOT NULL,
        [Status] varchar(30) NOT NULL,
        [StartedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [LastSavedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [SubmittedAtUtc] datetime2(0) NULL,
        [CurrentQuestionId] int NULL,
        [CurrentQuestionIndex] int NOT NULL,
        [TotalSecondsUsed] int NOT NULL,
        [TotalScore] decimal(10,2) NULL,
        [MaxScore] decimal(10,2) NULL,
        [CorrectCount] int NULL,
        [WrongCount] int NULL,
        [UnansweredCount] int NULL,
        [PercentageScore] decimal(5,2) NULL,
        [IsSubmitted] bit NOT NULL,
        CONSTRAINT [PK_ExamAttempts] PRIMARY KEY ([ExamAttemptId]),
        CONSTRAINT [CK_ExamAttempts_Score] CHECK ([TotalScore] IS NULL OR [TotalScore] >= 0),
        CONSTRAINT [CK_ExamAttempts_Status] CHECK ([Status] IN ('InProgress', 'Paused', 'Submitted', 'Expired', 'Cancelled')),
        CONSTRAINT [FK_ExamAttempts_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([ExamPaperId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamAttempts_Questions_CurrentQuestionId] FOREIGN KEY ([CurrentQuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ExamAttempts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [QuestionExplanations] (
        [ExplanationId] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        [ExplanationTitle] nvarchar(250) NULL,
        [ExplanationText] nvarchar(max) NOT NULL,
        [TranscriptText] nvarchar(max) NULL,
        [GrammarNote] nvarchar(max) NULL,
        [VocabularyNote] nvarchar(max) NULL,
        [ExplanationMediaUrl] nvarchar(1000) NULL,
        [IsPublished] bit NOT NULL,
        CONSTRAINT [PK_QuestionExplanations] PRIMARY KEY ([ExplanationId]),
        CONSTRAINT [FK_QuestionExplanations_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [QuestionMedia] (
        [MediaId] int NOT NULL IDENTITY,
        [QuestionId] int NULL,
        [QuestionGroupId] int NULL,
        [MediaType] varchar(20) NOT NULL,
        [FileUrl] nvarchar(1000) NOT NULL,
        [FileName] nvarchar(255) NULL,
        [MimeType] varchar(100) NULL,
        [FileSizeBytes] bigint NULL,
        [DisplayOrder] int NOT NULL,
        [DurationSeconds] int NULL,
        [AltText] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_QuestionMedia] PRIMARY KEY ([MediaId]),
        CONSTRAINT [CK_QuestionMedia_Owner] CHECK ([QuestionId] IS NOT NULL OR [QuestionGroupId] IS NOT NULL),
        CONSTRAINT [CK_QuestionMedia_Type] CHECK ([MediaType] IN ('Image', 'Audio', 'Video', 'Document')),
        CONSTRAINT [FK_QuestionMedia_QuestionGroups_QuestionGroupId] FOREIGN KEY ([QuestionGroupId]) REFERENCES [QuestionGroups] ([QuestionGroupId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_QuestionMedia_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [QuestionOptions] (
        [QuestionOptionId] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        [OptionCode] varchar(10) NULL,
        [OptionText] nvarchar(max) NOT NULL,
        [OptionOrder] int NOT NULL,
        [IsCorrect] bit NOT NULL,
        [MatchingKey] varchar(100) NULL,
        CONSTRAINT [PK_QuestionOptions] PRIMARY KEY ([QuestionOptionId]),
        CONSTRAINT [FK_QuestionOptions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [AttemptEvents] (
        [AttemptEventId] bigint NOT NULL IDENTITY,
        [ExamAttemptId] bigint NOT NULL,
        [EventType] varchar(50) NOT NULL,
        [EventDataJson] nvarchar(max) NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_AttemptEvents] PRIMARY KEY ([AttemptEventId]),
        CONSTRAINT [FK_AttemptEvents_ExamAttempts_ExamAttemptId] FOREIGN KEY ([ExamAttemptId]) REFERENCES [ExamAttempts] ([ExamAttemptId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [AttemptResults] (
        [AttemptResultId] bigint NOT NULL IDENTITY,
        [ExamAttemptId] bigint NOT NULL,
        [TotalScore] decimal(10,2) NOT NULL,
        [MaxScore] decimal(10,2) NOT NULL,
        [PercentageScore] decimal(5,2) NOT NULL,
        [CorrectCount] int NOT NULL,
        [WrongCount] int NOT NULL,
        [UnansweredCount] int NOT NULL,
        [StarsEarned] int NOT NULL,
        [ResultMessage] nvarchar(1000) NULL,
        [GradedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_AttemptResults] PRIMARY KEY ([AttemptResultId]),
        CONSTRAINT [CK_AttemptResults_Percentage] CHECK ([PercentageScore] BETWEEN 0 AND 100),
        CONSTRAINT [FK_AttemptResults_ExamAttempts_ExamAttemptId] FOREIGN KEY ([ExamAttemptId]) REFERENCES [ExamAttempts] ([ExamAttemptId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [StarTransactions] (
        [StarTransactionId] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ExamAttemptId] bigint NULL,
        [Amount] int NOT NULL,
        [TransactionType] varchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_StarTransactions] PRIMARY KEY ([StarTransactionId]),
        CONSTRAINT [FK_StarTransactions_ExamAttempts_ExamAttemptId] FOREIGN KEY ([ExamAttemptId]) REFERENCES [ExamAttempts] ([ExamAttemptId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StarTransactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE TABLE [AttemptAnswers] (
        [AttemptAnswerId] bigint NOT NULL IDENTITY,
        [ExamAttemptId] bigint NOT NULL,
        [QuestionId] int NOT NULL,
        [SelectedOptionId] int NULL,
        [TextAnswer] nvarchar(max) NULL,
        [AnswerJson] nvarchar(max) NULL,
        [IsAnswered] bit NOT NULL,
        [IsCorrect] bit NULL,
        [ScoreEarned] decimal(10,2) NULL,
        [TimeSpentSeconds] int NOT NULL,
        [SavedAtUtc] datetime2(0) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_AttemptAnswers] PRIMARY KEY ([AttemptAnswerId]),
        CONSTRAINT [FK_AttemptAnswers_ExamAttempts_ExamAttemptId] FOREIGN KEY ([ExamAttemptId]) REFERENCES [ExamAttempts] ([ExamAttemptId]) ON DELETE CASCADE,
        CONSTRAINT [FK_AttemptAnswers_QuestionOptions_SelectedOptionId] FOREIGN KEY ([SelectedOptionId]) REFERENCES [QuestionOptions] ([QuestionOptionId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttemptAnswers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Achievements_AchievementCode] ON [Achievements] ([AchievementCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AttemptAnswers_QuestionId] ON [AttemptAnswers] ([QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AttemptAnswers_SelectedOptionId] ON [AttemptAnswers] ([SelectedOptionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UQ_AttemptAnswers_AttemptQuestion] ON [AttemptAnswers] ([ExamAttemptId], [QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AttemptEvents_ExamAttemptId] ON [AttemptEvents] ([ExamAttemptId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AttemptResults_ExamAttemptId] ON [AttemptResults] ([ExamAttemptId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ExamAttempts_CurrentQuestionId] ON [ExamAttempts] ([CurrentQuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ExamAttempts_ExamPaperId] ON [ExamAttempts] ([ExamPaperId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ExamAttempts_User_Exam] ON [ExamAttempts] ([UserId], [ExamPaperId], [StartedAtUtc] DESC);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ExamAttempts_User_Status] ON [ExamAttempts] ([UserId], [Status], [LastSavedAtUtc] DESC);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamLevels_LevelCode] ON [ExamLevels] ([LevelCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ExamPapers_CreatedByUserId] ON [ExamPapers] ([CreatedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamPapers_ExamCode] ON [ExamPapers] ([ExamCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ExamPapers_Level_Published] ON [ExamPapers] ([LevelId], [IsPublished], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamSections_ExamPaperId_SectionCode] ON [ExamSections] ([ExamPaperId], [SectionCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ExamSections_ExamPaperId_SectionOrder] ON [ExamSections] ([ExamPaperId], [SectionOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Leaderboard_Period] ON [LeaderboardSnapshots] ([PeriodType], [PeriodStartDate], [LevelId], [RankNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LeaderboardSnapshots_LevelId] ON [LeaderboardSnapshots] ([LevelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_LeaderboardSnapshots_UserId] ON [LeaderboardSnapshots] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_QuestionExplanations_QuestionId] ON [QuestionExplanations] ([QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_QuestionGroups_ExamSectionId_GroupOrder] ON [QuestionGroups] ([ExamSectionId], [GroupOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_QuestionMedia_Question] ON [QuestionMedia] ([QuestionId], [DisplayOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_QuestionMedia_QuestionGroupId] ON [QuestionMedia] ([QuestionGroupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_QuestionOptions_QuestionId_OptionOrder] ON [QuestionOptions] ([QuestionId], [OptionOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Questions_QuestionGroupId] ON [Questions] ([QuestionGroupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Questions_QuestionTypeId] ON [Questions] ([QuestionTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Questions_Section_Order] ON [Questions] ([ExamSectionId], [QuestionOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_QuestionTypes_TypeCode] ON [QuestionTypes] ([TypeCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StarTransactions_ExamAttemptId] ON [StarTransactions] ([ExamAttemptId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StarTransactions_User_Date] ON [StarTransactions] ([UserId], [CreatedAtUtc] DESC);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserAchievements_AchievementId] ON [UserAchievements] ([AchievementId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserAchievements_UserId_AchievementId] ON [UserAchievements] ([UserId], [AchievementId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]) WHERE [NormalizedEmail] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserVocabulary_UserId_OriginalText] ON [UserVocabulary] ([UserId], [OriginalText]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002083447_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002083447_InitialCreate', N'8.0.31');
END;
GO

COMMIT;
GO

