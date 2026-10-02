namespace CambridgeExamSystem.Domain.Common;

public static class AttemptEventTypes
{
    public const string Started = "STARTED";
    public const string AnswerSaved = "ANSWER_SAVED";
    public const string AutoSaved = "AUTO_SAVED";
    public const string Paused = "PAUSED";
    public const string Resumed = "RESUMED";
    public const string Submitted = "SUBMITTED";
    public const string TimeExpired = "TIME_EXPIRED";
    public const string BrowserReconnected = "BROWSER_RECONNECTED";
}
