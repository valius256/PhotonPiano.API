using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.AccountFirebaseId);
        
        
        // reference
        builder.HasMany(x => x.EntranceTestStudents)
            .WithOne(x => x.LearnerAccount)
            .HasForeignKey(x => x.LearnerFirebaseId)
            .OnDelete(DeleteBehavior.NoAction); 
        
        builder.HasMany(x => x.EntranceTests)
            .WithOne(x => x.TeacherAccount)
            .HasForeignKey(x => x.TeacherFirebaseId)
            .OnDelete(DeleteBehavior.NoAction);
        
   
    }
}