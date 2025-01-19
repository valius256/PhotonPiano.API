using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class SlotConfiguration : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();


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
    }
}