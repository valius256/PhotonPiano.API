using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class SurveyQuestionConfiguration : IEntityTypeConfiguration<SurveyQuestion>
{
    public void Configure(EntityTypeBuilder<SurveyQuestion> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(x => x.RecordStatus != RecordStatus.IsDeleted);

        builder.HasMany(x => x.LearnerSurveys)
            .WithOne(x => x.SurveyQuestion)
            .HasForeignKey(x => x.SurveyQuestionId)
            .OnDelete(DeleteBehavior.NoAction)
            ;

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedSurveyQuestions)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction)
            ;

        builder.HasOne(x => x.UpdatedBy)
            .WithMany(x => x.UpdatedSurveyQuestions)
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.NoAction)
            ;
    }
}