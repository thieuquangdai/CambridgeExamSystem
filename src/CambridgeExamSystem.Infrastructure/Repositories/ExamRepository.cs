using CambridgeExamSystem.Application.Interfaces.Repositories;
using CambridgeExamSystem.Domain.Entities;
using CambridgeExamSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Repositories;

public sealed class ExamRepository(CambridgeDbContext context) : IExamRepository
{
    public Task<List<ExamLevel>> GetLevelsAsync(CancellationToken cancellationToken = default) =>
        context.ExamLevels
            .AsNoTracking()
            .Include(l => l.ExamPapers)
            .Where(l => l.IsActive)
            .OrderBy(l => l.DisplayOrder)
            .ToListAsync(cancellationToken);

    public Task<ExamLevel?> GetLevelAsync(int levelId, CancellationToken cancellationToken = default) =>
        context.ExamLevels.FirstOrDefaultAsync(l => l.LevelId == levelId, cancellationToken);

    public Task<List<ExamPaper>> GetPapersAsync(string? levelCode, bool publishedOnly, CancellationToken cancellationToken = default)
    {
        var query = context.ExamPapers
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Level)
            .Include(p => p.Sections).ThenInclude(s => s.Questions)
            .Where(p => p.IsActive);

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        if (!string.IsNullOrWhiteSpace(levelCode))
        {
            query = query.Where(p => p.Level.LevelCode == levelCode);
        }

        return query.OrderBy(p => p.Level.DisplayOrder).ThenBy(p => p.ExamName).ToListAsync(cancellationToken);
    }

    public Task<ExamPaper?> GetPaperAsync(int examPaperId, CancellationToken cancellationToken = default) =>
        context.ExamPapers.Include(p => p.Level).FirstOrDefaultAsync(p => p.ExamPaperId == examPaperId && p.IsActive, cancellationToken);

    public Task<ExamPaper?> GetPaperWithContentAsync(int examPaperId, CancellationToken cancellationToken = default) =>
        context.ExamPapers
            .AsSplitQuery()
            .Include(p => p.Level)
            .Include(p => p.Sections).ThenInclude(s => s.QuestionGroups).ThenInclude(g => g.Media)
            .Include(p => p.Sections).ThenInclude(s => s.Questions).ThenInclude(q => q.QuestionType)
            .Include(p => p.Sections).ThenInclude(s => s.Questions).ThenInclude(q => q.Options)
            .Include(p => p.Sections).ThenInclude(s => s.Questions).ThenInclude(q => q.Media)
            .Include(p => p.Sections).ThenInclude(s => s.Questions).ThenInclude(q => q.Explanations)
            .FirstOrDefaultAsync(p => p.ExamPaperId == examPaperId && p.IsActive, cancellationToken);

    public Task<bool> ExamCodeExistsAsync(string examCode, int? excludeExamPaperId, CancellationToken cancellationToken = default) =>
        context.ExamPapers.AnyAsync(p => p.ExamCode == examCode && p.ExamPaperId != excludeExamPaperId, cancellationToken);

    public Task<List<QuestionType>> GetQuestionTypesAsync(CancellationToken cancellationToken = default) =>
        context.QuestionTypes.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.QuestionTypeId).ToListAsync(cancellationToken);

    public Task<QuestionType?> GetQuestionTypeAsync(int questionTypeId, CancellationToken cancellationToken = default) =>
        context.QuestionTypes.FirstOrDefaultAsync(t => t.QuestionTypeId == questionTypeId && t.IsActive, cancellationToken);

    public Task<Section?> GetSectionWithQuestionsAsync(int examSectionId, CancellationToken cancellationToken = default) =>
        context.Sections
            .Include(s => s.ExamPaper)
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.ExamSectionId == examSectionId, cancellationToken);

    public Task<Question?> GetQuestionAsync(int questionId, CancellationToken cancellationToken = default) =>
        context.Questions
            .Include(q => q.Section).ThenInclude(s => s.ExamPaper)
            .Include(q => q.QuestionType)
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.QuestionId == questionId, cancellationToken);

    public Task<int> CountActiveQuestionsAsync(CancellationToken cancellationToken = default) =>
        context.Questions.CountAsync(q => q.IsActive && q.Section.ExamPaper.IsActive, cancellationToken);

    public void AddPaper(ExamPaper paper) => context.ExamPapers.Add(paper);

    public void AddSection(Section section) => context.Sections.Add(section);

    public void AddQuestion(Question question) => context.Questions.Add(question);
}
