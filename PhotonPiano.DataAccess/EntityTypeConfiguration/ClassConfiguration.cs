using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.Instructor)
            .WithMany(x => x.InstructorClasses)
            .HasForeignKey(x => x.InstructorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedClasses)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedClasses)
            .HasForeignKey(x => x.UpdateById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedClasses)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}