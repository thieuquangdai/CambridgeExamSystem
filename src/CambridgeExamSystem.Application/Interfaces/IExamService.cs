using CambridgeExamSystem.Application.Common;
using CambridgeExamSystem.Application.DTOs;

namespace CambridgeExamSystem.Application.Interfaces;

public interface IExamService
{
    Task<List<ExamLevelDto>> GetLevelsAsync(CancellationToken cancellationToken = default);
    Task<List<ExamPaperSummaryDto>> GetPublishedExamsAsync(string? levelCode, int? userId, CancellationToken cancellationToken = default);
    Task<ExamPaperDetailDto?> GetExamDetailAsync(int examPaperId, int? userId, CancellationToken cancellationToken = default);

    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<List<ExamPaperSummaryDto>> GetAllExamsAsync(CancellationToken cancellationToken = default);
    Task<ExamPaperEditDto?> GetExamForEditAsync(int examPaperId, CancellationToken cancellationToken = default);
    Task<ExamContentDto?> GetExamContentAsync(int examPaperId, CancellationToken cancellationToken = default);
    Task<List<QuestionTypeDto>> GetQuestionTypesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> SaveExamAsync(ExamPaperEditDto dto, int? createdByUserId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> SetPublishedAsync(int examPaperId, bool isPublished, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> AddSectionAsync(SectionEditDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> AddQuestionAsync(QuestionEditDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> DeactivateQuestionAsync(int questionId, CancellationToken cancellationToken = default);
}
