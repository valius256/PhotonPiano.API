using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(ap => ap.Id);

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

        builder.Property(ap => ap.AdditionalData).HasColumnType("json");

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedApplications)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UpdatedBy)
            .WithMany(x => x.UpdatedApplications)
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApprovedBy)
            .WithMany(x => x.ApprovedApplications)
            .HasForeignKey(x => x.ApprovedById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedApplications)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.Cascade);
    }
}