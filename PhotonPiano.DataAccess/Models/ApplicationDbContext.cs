using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhotonPiano.DataAccess.EntityTypeConfiguration;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.SeedData;

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
    public DbSet<Tution> Tutions { get; set; }
    public DbSet<New> News { get; set; }
    public DbSet<DayOff> DayOffs { get; set; }


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
        modelBuilder.ApplyConfiguration(new TutionConfiguration());
        modelBuilder.ApplyConfiguration(new NewConfiguration());
        modelBuilder.ApplyConfiguration(new DayOffConfiguration());


        foreach (var entity in modelBuilder.Model.GetEntityTypes()) entity.SetTableName(entity.DisplayName());


        modelBuilder.Seed();
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