using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Interfaces.Repositories;

public interface ITestAttemptRepository
{
    Task<TestAttempt?> GetOpenAttemptAsync(int userId, int examPaperId, CancellationToken cancellationToken = default);
    Task<List<TestAttempt>> GetOpenAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task<TestAttempt?> GetWithAnswersAsync(long examAttemptId, int userId, CancellationToken cancellationToken = default);
    Task<int> CountAttemptsAsync(int userId, int examPaperId, CancellationToken cancellationToken = default);
    Task<List<TestAttempt>> GetUserAttemptsAsync(int userId, int? examPaperId, CancellationToken cancellationToken = default);
    Task<(int Total, int Submitted, decimal AveragePercentage)> GetAttemptTotalsAsync(CancellationToken cancellationToken = default);
    void Add(TestAttempt attempt);
    void AddEvent(AttemptEvent attemptEvent);
}
