using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class StudentClassConfiguration : IEntityTypeConfiguration<StudentClass>
{
    public void Configure(EntityTypeBuilder<StudentClass> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(q => q.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.Student)
            .WithMany(x => x.StudentClasses)
            .HasForeignKey(x => x.StudentFirebaseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Class)
            .WithMany(x => x.StudentClasses)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedStudentClass)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedStudentClass)
            .HasForeignKey(x => x.UpdateById)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedStudentClass)
            .HasForeignKey(x => x.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}