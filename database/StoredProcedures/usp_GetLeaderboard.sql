-- Reporting helper (the web application computes leaderboards through EF Core).
USE [CambridgeExamDb];
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GetLeaderboard]
    @Top int = 50,
    @LevelCode varchar(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@Top)
        RANK() OVER (ORDER BY s.[TotalStars] DESC, s.[AverageScore] DESC, s.[TotalTests] DESC) AS [Rank],
        u.[UserId],
        u.[FullName],
        u.[CurrentLevelCode],
        s.[TotalStars],
        s.[AverageScore],
        s.[TotalTests],
        s.[CurrentRankCode]
    FROM [UserStatistics] s
    INNER JOIN [Users] u ON u.[UserId] = s.[UserId]
    WHERE u.[IsActive] = 1
      AND s.[TotalTests] > 0
      AND (@LevelCode IS NULL OR u.[CurrentLevelCode] = @LevelCode)
    ORDER BY [Rank], u.[FullName];
END;
GO
