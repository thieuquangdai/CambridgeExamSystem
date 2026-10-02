using AutoMapper;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ExamLevel, ExamLevelDto>()
            .ForMember(d => d.PublishedExamCount, o => o.MapFrom(s => s.ExamPapers.Count(p => p.IsPublished && p.IsActive)));

        CreateMap<ExamPaper, ExamPaperSummaryDto>()
            .ForMember(d => d.LevelCode, o => o.MapFrom(s => s.Level.LevelCode))
            .ForMember(d => d.LevelName, o => o.MapFrom(s => s.Level.LevelName))
            .ForMember(d => d.SectionCount, o => o.MapFrom(s => s.Sections.Count))
            .ForMember(d => d.QuestionCount, o => o.MapFrom(s => s.Sections.SelectMany(x => x.Questions).Count(q => q.IsActive)))
            .ForMember(d => d.ActiveAttemptId, o => o.Ignore())
            .ForMember(d => d.BestPercentage, o => o.Ignore())
            .ForMember(d => d.AttemptCount, o => o.Ignore());

        CreateMap<ExamPaper, ExamPaperEditDto>();

        CreateMap<Section, SectionSummaryDto>()
            .ForMember(d => d.QuestionCount, o => o.MapFrom(s => s.Questions.Count(q => q.IsActive)));

        CreateMap<QuestionType, QuestionTypeDto>();

        CreateMap<QuestionMedia, MediaDto>()
            .ForMember(d => d.MediaType, o => o.MapFrom(s => s.MediaType.ToString()));

        CreateMap<QuestionGroup, TestGroupDto>()
            .ForMember(d => d.Media, o => o.MapFrom(s => s.Media.Where(m => m.IsActive).OrderBy(m => m.DisplayOrder)));

        CreateMap<AnswerOption, TestOptionDto>();

        CreateMap<Question, TestQuestionDto>()
            .ForMember(d => d.TypeCode, o => o.MapFrom(s => s.QuestionType.TypeCode))
            .ForMember(d => d.Media, o => o.MapFrom(s => s.Media.Where(m => m.IsActive).OrderBy(m => m.DisplayOrder)))
            .ForMember(d => d.Options, o => o.Ignore())
            .ForMember(d => d.MatchingKeys, o => o.Ignore())
            .ForMember(d => d.Index, o => o.Ignore());

        CreateMap<AnswerOption, OptionAdminDto>();

        CreateMap<Question, QuestionAdminDto>()
            .ForMember(d => d.TypeCode, o => o.MapFrom(s => s.QuestionType.TypeCode))
            .ForMember(d => d.TypeName, o => o.MapFrom(s => s.QuestionType.TypeName))
            .ForMember(d => d.Options, o => o.MapFrom(s => s.Options.OrderBy(x => x.OptionOrder)));

        CreateMap<TestAnswer, SavedAnswerDto>();

        CreateMap<TestAttempt, AttemptHistoryItemDto>()
            .ForMember(d => d.ExamName, o => o.MapFrom(s => s.ExamPaper.ExamName))
            .ForMember(d => d.LevelName, o => o.MapFrom(s => s.ExamPaper.Level.LevelName))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.StarsEarned, o => o.MapFrom(s => s.Result == null ? 0 : s.Result.StarsEarned));

        CreateMap<Achievement, AchievementDto>()
            .ForMember(d => d.IsEarned, o => o.Ignore())
            .ForMember(d => d.EarnedAtUtc, o => o.Ignore());

        CreateMap<UserStatistic, UserStatisticsDto>();

        CreateMap<UserVocabulary, VocabularyDto>();
    }
}
