using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class EntranceTestConfiguration : IEntityTypeConfiguration<EntranceTest>
{
    public void Configure(EntityTypeBuilder<EntranceTest> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.Instructor)
            .WithMany(x => x.InstructorEntranceTests)
            .HasForeignKey(x => x.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedEntrancesTest)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedEntrancesTest)
            .HasForeignKey(x => x.UpdateById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedEntrancesTest)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}