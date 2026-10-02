using CambridgeExamSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CambridgeExamSystem.Infrastructure.Data;

public class CambridgeDbContext(DbContextOptions<CambridgeDbContext> options)
    : IdentityDbContext<User, Role, int>(options)
{
    public DbSet<ExamLevel> ExamLevels => Set<ExamLevel>();
    public DbSet<ExamPaper> ExamPapers => Set<ExamPaper>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();
    public DbSet<QuestionGroup> QuestionGroups => Set<QuestionGroup>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<QuestionMedia> QuestionMedia => Set<QuestionMedia>();
    public DbSet<Explanation> Explanations => Set<Explanation>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<TestAnswer> TestAnswers => Set<TestAnswer>();
    public DbSet<AttemptEvent> AttemptEvents => Set<AttemptEvent>();
    public DbSet<AttemptResult> AttemptResults => Set<AttemptResult>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<UserStatistic> UserStatistics => Set<UserStatistic>();
    public DbSet<StarTransaction> StarTransactions => Set<StarTransaction>();
    public DbSet<LeaderboardSnapshot> LeaderboardSnapshots => Set<LeaderboardSnapshot>();
    public DbSet<UserVocabulary> UserVocabulary => Set<UserVocabulary>();
    public DbSet<MotivationalMessage> MotivationalMessages => Set<MotivationalMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        builder.ApplyConfigurationsFromAssembly(typeof(CambridgeDbContext).Assembly);
    }
}
