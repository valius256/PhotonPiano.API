using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class SlotStudentConfiguration : IEntityTypeConfiguration<SlotStudent>
{
    public void Configure(EntityTypeBuilder<SlotStudent> builder)
    {
        builder.HasKey(x => new
        {
            x.SlotId,
            x.StudentFirebaseId
        }
        );

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);


        builder.HasOne(x => x.StudentAccount)
            .WithMany(x => x.SlotStudents)
            .HasForeignKey(x => x.StudentFirebaseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Slot)
            .WithMany(x => x.SlotStudents)
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreateBy)
            .WithMany(x => x.CreatedSlotStudents)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedSlotStudents)
            .HasForeignKey(x => x.UpdateById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedSlotStudents)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}