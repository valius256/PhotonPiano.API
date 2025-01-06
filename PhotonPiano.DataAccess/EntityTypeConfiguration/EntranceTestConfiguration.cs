using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class EntranceTestConfiguration : IEntityTypeConfiguration<EntranceTest>
{
    public void Configure(EntityTypeBuilder<EntranceTest> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        
    }
}