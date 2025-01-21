using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class AccountNotificationConfiguration : IEntityTypeConfiguration<AccountNotification>
{
    public void Configure(EntityTypeBuilder<AccountNotification> builder)
    {
        builder.HasKey(an => new { an.AccountFirebaseId, an.NotificationId });
        builder.HasOne(x => x.Account)
            .WithMany(x => x.AccountNotifications)
            .HasForeignKey(x => x.AccountFirebaseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Notification)
            .WithMany(x => x.AccountNotifications)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}