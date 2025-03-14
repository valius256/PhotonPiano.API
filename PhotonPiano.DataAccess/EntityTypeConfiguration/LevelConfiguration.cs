

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using System.Reflection.Emit;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
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

            builder.HasMany(l => l.EntranceTestStudents)
               .WithOne(ets => ets.Level)
               .HasForeignKey(ets => ets.LevelId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
        }
    }
}
