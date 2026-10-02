using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Application.Interfaces;

public interface ITestService
{
    Task<long> StartOrResumeAsync(int userId, int examPaperId, CancellationToken cancellationToken = default);
    Task<TakeTestDto?> GetTakeTestAsync(long examAttemptId, int userId, CancellationToken cancellationToken = default);
    Task<SaveAnswerResult> SaveAnswerAsync(int userId, SaveAnswerRequest request, CancellationToken cancellationToken = default);
    Task PauseAsync(int userId, long examAttemptId, int currentQuestionIndex, int timeSpentSeconds, CancellationToken cancellationToken = default);
    Task<long> SubmitAsync(int userId, long examAttemptId, CancellationToken cancellationToken = default);
    Task<AttemptResultDto?> GetResultAsync(long examAttemptId, int userId, CancellationToken cancellationToken = default);
    Task<List<AttemptHistoryItemDto>> GetHistoryAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<ActiveAttemptDto>> GetActiveAttemptsAsync(int userId, CancellationToken cancellationToken = default);
}
