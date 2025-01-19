using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class StudentClassScoreConfiguration : IEntityTypeConfiguration<StudentClassScore>
{
    public void Configure(EntityTypeBuilder<StudentClassScore> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.Criteria)
            .WithMany(x => x.StudentClassScoreCriterias)
            .HasForeignKey(x => x.CriteriaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.StudentClass)
            .WithMany(x => x.StudentClassScores)
            .HasForeignKey(x => x.StudentClassId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}