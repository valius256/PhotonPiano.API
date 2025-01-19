using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();


        builder.Property(x => x.Content).IsRequired();


        builder.HasOne(x => x.Receiver)
            .WithMany(x => x.ReceiverNotifications)
            .HasForeignKey(x => x.ReceiverFirebaseId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Sender)
            .WithMany(x => x.SenderNotifications)
            .HasForeignKey(x => x.SenderFirebaseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}