using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class CriteriaConfiguration : IEntityTypeConfiguration<Criteria>
{
    public void Configure(EntityTypeBuilder<Criteria> builder)
    {
        builder.HasIndex(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        

    }
}