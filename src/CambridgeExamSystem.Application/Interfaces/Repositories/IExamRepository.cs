using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Interfaces.Repositories;

public interface IExamRepository
{
    Task<List<ExamLevel>> GetLevelsAsync(CancellationToken cancellationToken = default);
    Task<ExamLevel?> GetLevelAsync(int levelId, CancellationToken cancellationToken = default);
    Task<List<ExamPaper>> GetPapersAsync(string? levelCode, bool publishedOnly, CancellationToken cancellationToken = default);
    Task<ExamPaper?> GetPaperAsync(int examPaperId, CancellationToken cancellationToken = default);
    Task<ExamPaper?> GetPaperWithContentAsync(int examPaperId, CancellationToken cancellationToken = default);
    Task<bool> ExamCodeExistsAsync(string examCode, int? excludeExamPaperId, CancellationToken cancellationToken = default);
    Task<List<QuestionType>> GetQuestionTypesAsync(CancellationToken cancellationToken = default);
    Task<QuestionType?> GetQuestionTypeAsync(int questionTypeId, CancellationToken cancellationToken = default);
    Task<Section?> GetSectionWithQuestionsAsync(int examSectionId, CancellationToken cancellationToken = default);
    Task<Question?> GetQuestionAsync(int questionId, CancellationToken cancellationToken = default);
    Task<int> CountActiveQuestionsAsync(CancellationToken cancellationToken = default);
    void AddPaper(ExamPaper paper);
    void AddSection(Section section);
    void AddQuestion(Question question);
}
