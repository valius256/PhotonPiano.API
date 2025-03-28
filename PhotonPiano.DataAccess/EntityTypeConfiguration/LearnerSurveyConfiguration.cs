using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class LearnerSurveyConfiguration : IEntityTypeConfiguration<LearnerSurvey>
{
    public void Configure(EntityTypeBuilder<LearnerSurvey> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(x => x.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.Learner)
            .WithMany(x => x.LearnerSurveys)
            .HasForeignKey(x => x.LearnerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PianoSurvey)
            .WithMany(ps => ps.LearnerSurveys)
            .HasForeignKey(x => x.PianoSurveyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}