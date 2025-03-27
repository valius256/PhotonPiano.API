using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhotonPiano.DataAccess.EntityTypeConfiguration;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Models;

public class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Criteria> Criteria { get; set; }
    public DbSet<EntranceTest> EntranceTests { get; set; }
    public DbSet<EntranceTestResult> EntranceTestResults { get; set; }
    public DbSet<EntranceTestStudent> EntranceTestStudents { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<SlotStudent> SlotStudents { get; set; }
    public DbSet<StudentClass> StudentClasses { get; set; }
    public DbSet<StudentClassScore> StudentClassScores { get; set; }
    public DbSet<SystemConfig> SystemConfigs { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Tuition> Tuitions { get; set; }
    public DbSet<New> News { get; set; }
    public DbSet<DayOff> DayOffs { get; set; }
    public DbSet<AccountNotification> AccountNotifications { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<PianoSurvey> PianoSurveys { get; set; }
    public DbSet<LearnerSurvey> LearnerSurveys { get; set; }
    public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
    public DbSet<LearnerAnswer> LearnerAnswers { get; set; }
    public DbSet<PianoSurveyQuestion> PianoSurveyQuestions { get; set; }
    public DbSet<Level> Levels { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new CriteriaConfiguration());
        modelBuilder.ApplyConfiguration(new EntranceTestConfiguration());
        modelBuilder.ApplyConfiguration(new EntranceTestResultConfiguration());
        modelBuilder.ApplyConfiguration(new EntranceTestStudentConfiguration());
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new ClassConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new SlotConfiguration());
        modelBuilder.ApplyConfiguration(new SlotStudentConfiguration());
        modelBuilder.ApplyConfiguration(new StudentClassConfiguration());
        modelBuilder.ApplyConfiguration(new StudentClassScoreConfiguration());
        modelBuilder.ApplyConfiguration(new SystemConfigConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new TuitionConfiguration());
        modelBuilder.ApplyConfiguration(new NewConfiguration());
        modelBuilder.ApplyConfiguration(new DayOffConfiguration());
        modelBuilder.ApplyConfiguration(new AccountNotificationConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
        modelBuilder.ApplyConfiguration(new PianoSurveyConfiguration());
        modelBuilder.ApplyConfiguration(new LearnerSurveyConfiguration());
        modelBuilder.ApplyConfiguration(new SurveyQuestionConfiguration());
        modelBuilder.ApplyConfiguration(new LearnerAnswerConfiguration());
        modelBuilder.ApplyConfiguration(new PianoSurveyQuestionConfiguration());
        modelBuilder.ApplyConfiguration(new LevelConfiguration());

        foreach (var entity in modelBuilder.Model.GetEntityTypes()) entity.SetTableName(entity.DisplayName());

        //modelBuilder.Seed();

        base.OnModelCreating(modelBuilder);
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            .ConfigureWarnings(waring => waring.Ignore(CoreEventId.AccidentalEntityType))
            ;
    }
}