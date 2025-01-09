using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);  
        builder.Property(r => r.Id).ValueGeneratedOnAdd();  
        
       builder.HasMany(x => x.EntranceTests)
           .WithOne(r => r.Room)
           .HasForeignKey(r => r.RoomId)
           .OnDelete(DeleteBehavior.NoAction)
           ;
       
       builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);
        
       builder.HasOne(x => x.CreatedBy)
           .WithMany(x => x.CreatedRoom)
           .HasForeignKey(x => x.CreatedById)
           .OnDelete(DeleteBehavior.Cascade);
        
       builder.HasOne(x => x.UpdateBy)
           .WithMany(x => x.UpdatedRoom)
           .HasForeignKey(x => x.UpdateById)
           .OnDelete(DeleteBehavior.NoAction);
        
       builder.HasOne(x => x.DeletedBy)
           .WithMany(x => x.DeletedRoom)
           .HasForeignKey(x => x.DeletedById)
           .OnDelete(DeleteBehavior.NoAction);

       
        
    }
}