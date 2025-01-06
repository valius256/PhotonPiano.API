using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class EntranceTestResultConfiguration : IEntityTypeConfiguration<EntranceTestResult>
{
    public void Configure(EntityTypeBuilder<EntranceTestResult> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
       builder.HasOne(x => x.Criteria).WithMany(x => x.EntranceTestResults).HasForeignKey(x => x.CriteriaId);
    }
}