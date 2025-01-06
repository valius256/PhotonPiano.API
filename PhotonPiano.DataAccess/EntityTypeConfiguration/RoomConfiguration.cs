using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);  
        builder.Property(r => r.Id).ValueGeneratedOnAdd();  
        
       builder.HasMany(x => x.EntranceTests).WithOne(r => r.Room).HasForeignKey(r => r.RoomId)
           .OnDelete(DeleteBehavior.NoAction)
           ;
       
        
    }
}