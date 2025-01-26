using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class NewConfiguration : IEntityTypeConfiguration<New>
{
    public void Configure(EntityTypeBuilder<New> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);


        builder.HasOne(x => x.CreateBy)
            .WithMany(x => x.CreatedNews)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatednNews)
            .HasForeignKey(x => x.UpdateById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletednNews)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}