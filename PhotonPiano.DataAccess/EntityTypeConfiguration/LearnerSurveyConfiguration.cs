using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class LearnerSurveyConfiguration : IEntityTypeConfiguration<LearnerSurvey>
{
    public void Configure(EntityTypeBuilder<LearnerSurvey> builder)
    {
        builder.HasKey(x => new { x.LearnerId, x.SurveyQuestionId });

        builder.HasQueryFilter(x => x.RecordStatus != RecordStatus.IsDeleted);

        builder.Property(x => x.AllowMultipleAnswers)
            .HasDefaultValue(false);

        builder.HasOne(x => x.SurveyQuestion)
            .WithMany(x => x.LearnerSurveys)
            .HasForeignKey(x => x.SurveyQuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.LearnerSurveys)
            .HasForeignKey(x => x.LearnerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}