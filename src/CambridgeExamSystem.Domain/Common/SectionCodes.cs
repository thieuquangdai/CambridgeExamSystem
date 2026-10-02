namespace CambridgeExamSystem.Domain.Common;

public static class SectionCodes
{
    public const string Listening = "LISTENING";
    public const string Reading = "READING";
    public const string Writing = "WRITING";
    public const string Speaking = "SPEAKING";

    public static readonly string[] All = [Listening, Reading, Writing, Speaking];
}
