using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class EntranceTestResultConfiguration : IEntityTypeConfiguration<EntranceTestResult>
{
    public void Configure(EntityTypeBuilder<EntranceTestResult> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
       builder.HasOne(x => x.Criteria)
           .WithMany(x => x.EntranceTestResults)
           .HasForeignKey(x => x.CriteriaId)
           .OnDelete(DeleteBehavior.Cascade);
        
       builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);
        
       builder.HasOne(x => x.CreatedBy)
           .WithMany(x => x.CreatedEntranceTestResult)
           .HasForeignKey(x => x.CreatedById)
           .OnDelete(DeleteBehavior.Cascade);
        
       builder.HasOne(x => x.UpdateBy)
           .WithMany(x => x.UpdatedEntranceTestResult)
           .HasForeignKey(x => x.UpdateById)
           .OnDelete(DeleteBehavior.NoAction);
        
       builder.HasOne(x => x.DeletedBy)
           .WithMany(x => x.DeletedEntranceTestResult)
           .HasForeignKey(x => x.DeletedById)
           .OnDelete(DeleteBehavior.NoAction);

       
       builder.HasOne(x => x.EntranceTestStudent)
           .WithMany(x => x.EntranceTestResults)
           .HasForeignKey(x => x.EntranceTestStudentId)
           .OnDelete(DeleteBehavior.Cascade);
        
    }
}