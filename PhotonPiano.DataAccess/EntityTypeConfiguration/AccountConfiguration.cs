using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.AccountFirebaseId);

        
        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);
        
        // reference
        builder.HasMany(x => x.EntranceTestStudents)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentFirebaseId)
            .OnDelete(DeleteBehavior.NoAction); 
        
        builder.HasMany(x => x.InstructorEntranceTests)
            .WithOne(x => x.Instructor)
            .HasForeignKey(x => x.InstructorId)
            .OnDelete(DeleteBehavior.NoAction);
        
        
    }
}