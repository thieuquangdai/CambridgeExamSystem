-- Reporting helper (the web application reads history through EF Core).
USE [CambridgeExamDb];
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetUserExamHistory]
    @UserId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.[ExamAttemptId],
        p.[ExamCode],
        p.[ExamName],
        l.[LevelName],
        a.[AttemptNumber],
        a.[Status],
        a.[StartedAtUtc],
        a.[SubmittedAtUtc],
        a.[TotalSecondsUsed],
        r.[TotalScore],
        r.[MaxScore],
        r.[PercentageScore],
        r.[StarsEarned]
    FROM [ExamAttempts] a
    INNER JOIN [ExamPapers] p ON p.[ExamPaperId] = a.[ExamPaperId]
    INNER JOIN [ExamLevels] l ON l.[LevelId] = p.[LevelId]
    LEFT JOIN [AttemptResults] r ON r.[ExamAttemptId] = a.[ExamAttemptId]
    WHERE a.[UserId] = @UserId
    ORDER BY a.[StartedAtUtc] DESC;
END;
GO
