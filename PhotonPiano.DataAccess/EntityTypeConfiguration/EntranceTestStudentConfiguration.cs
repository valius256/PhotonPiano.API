using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class EntranceTestStudentConfiguration : IEntityTypeConfiguration<EntranceTestStudent>
{
    public void Configure(EntityTypeBuilder<EntranceTestStudent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.EntranceTest)
                .WithMany(x => x.EntranceTestStudents)
                .HasForeignKey(x => x.EntranceTestId)
                .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);
        
        builder.HasOne(x => x.CreateBy)
            .WithMany(x => x.CreatedEntranceTestStudent)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedEntranceTestStudent)
            .HasForeignKey(x => x.UpdateById)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedEntranceTestStudent)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
   
    }
}