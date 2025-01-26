using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class TutionConfiguration : IEntityTypeConfiguration<Tution>
{
    public void Configure(EntityTypeBuilder<Tution> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.StudentClass)
            .WithMany(x => x.Tutions)
            .HasForeignKey(x => x.StudentClassId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}