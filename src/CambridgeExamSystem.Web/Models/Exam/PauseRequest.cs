namespace CambridgeExamSystem.Web.Models.Exam;

public class PauseRequest
{
    public long ExamAttemptId { get; set; }
    public int CurrentQuestionIndex { get; set; }
    public int TimeSpentSeconds { get; set; }
}
