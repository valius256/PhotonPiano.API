using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class EntranceTestStudentConfiguration : IEntityTypeConfiguration<EntranceTestStudent>
{
    public void Configure(EntityTypeBuilder<EntranceTestStudent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.EntranceTest)
                .WithMany(x => x.EntranceTestStudents)
                .HasForeignKey(x => x.EntranceTestId);
        
        
    }
}