namespace CambridgeExamSystem.Web.Infrastructure;

public static class DisplayFormat
{
    public static string Duration(long seconds)
    {
        var span = TimeSpan.FromSeconds(Math.Max(0, seconds));
        return span.TotalHours >= 1 ? $"{(int)span.TotalHours}:{span.Minutes:00}:{span.Seconds:00}" : $"{span.Minutes:00}:{span.Seconds:00}";
    }

    public static string Percent(decimal? value) => value is null ? "—" : $"{value:0.##}%";

    public static string Score(decimal? value) => value is null ? "—" : $"{value:0.##}";

    public static string LocalTime(DateTime? utc) => utc is null ? "—" : DateTime.SpecifyKind(utc.Value, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ");

    public static string RankBadge(string rankCode) => rankCode switch
    {
        "Diamond" => "bg-info text-dark",
        "Platinum" => "bg-secondary",
        "Gold" => "bg-warning text-dark",
        "Silver" => "bg-light text-dark border",
        _ => "bg-bronze"
    };

    public static string SectionBadge(string sectionCode) => sectionCode switch
    {
        "LISTENING" => "bg-primary",
        "READING" => "bg-success",
        "WRITING" => "bg-warning text-dark",
        "SPEAKING" => "bg-danger",
        _ => "bg-secondary"
    };
}
