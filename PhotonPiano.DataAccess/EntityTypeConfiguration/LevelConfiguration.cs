

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
            builder.Property(x => x.MinimumPracticalScore).HasDefaultValue(0.0);

            builder.Property(x => x.MinimumTheoreticalScore).HasDefaultValue(0.0);

            builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

            builder.HasOne(l => l.NextLevel)
                .WithOne() // No inverse navigation needed
                .HasForeignKey<Level>(l => l.NextLevelId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            builder.HasMany(l => l.Classes)
                .WithOne(c => c.Level)
                .HasForeignKey(c => c.LevelId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            builder.HasMany(l => l.Accounts)
               .WithOne(a => a.Level)
               .HasForeignKey(a => a.LevelId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
            
            builder.HasMany(l => l.SelfEvaluatedAccounts)
                .WithOne(a => a.SelfEvaluatedLevel)
                .HasForeignKey(a => a.SelfEvaluatedLevelId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes


            builder.HasMany(l => l.EntranceTestStudents)
               .WithOne(ets => ets.Level)
               .HasForeignKey(ets => ets.LevelId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
        }
    }
}
