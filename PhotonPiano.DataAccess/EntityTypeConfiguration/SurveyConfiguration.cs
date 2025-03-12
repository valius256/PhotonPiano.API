using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
{
    public void Configure(EntityTypeBuilder<Survey> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasQueryFilter(x => x.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.CreateBy)
            .WithMany(x => x.CreatedSurveys)
            .HasForeignKey(x => x.CreateById);
        
        builder.HasOne(x => x.UpdateBy)
            .WithMany(x => x.UpdatedSurveys)
            .HasForeignKey(x => x.UpdateById);
    }
}