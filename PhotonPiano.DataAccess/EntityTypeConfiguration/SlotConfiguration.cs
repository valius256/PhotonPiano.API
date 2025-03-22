using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class SlotConfiguration : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);


        builder.HasOne(x => x.Class)
            .WithMany(x => x.Slots)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Class)
            .WithMany(x => x.Slots)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Slots)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedSlots)
            .HasForeignKey(x => x.UpdateById);
        
        builder.HasOne(x => x.CancelBy)
            .WithMany(x => x.CanceledSlots)
            .HasForeignKey(x => x.CancelById);
         
    }
}