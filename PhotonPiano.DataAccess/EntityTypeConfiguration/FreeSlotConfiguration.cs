
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration
{
    public class FreeSlotConfiguration : IEntityTypeConfiguration<FreeSlot>
    {
        public void Configure(EntityTypeBuilder<FreeSlot> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

            builder.HasOne(x => x.Account)
                .WithMany(x => x.FreeSlots)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
