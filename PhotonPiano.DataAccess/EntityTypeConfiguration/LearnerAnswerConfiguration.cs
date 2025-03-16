using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class LearnerAnswerConfiguration : IEntityTypeConfiguration<LearnerAnswer>
{
    public void Configure(EntityTypeBuilder<LearnerAnswer> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasQueryFilter(x => x.RecordStatus != RecordStatus.IsDeleted);

        builder.HasOne(x => x.LearnerSurvey)
            .WithMany(ls => ls.LearnerAnswers)
            .HasForeignKey(x => x.LearnerSurveyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.SurveyQuestion)
            .WithMany(ls => ls.LearnerAnswers)
            .HasForeignKey(x => x.SurveyQuestionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}