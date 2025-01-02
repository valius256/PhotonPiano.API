using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhotonPiano.DataAccess.EntityTypeConfiguration;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.SeedData;

namespace PhotonPiano.DataAccess.Models
{
    public class ApplicationDbContext : DbContext
    {
        protected ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());



            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }


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
}
