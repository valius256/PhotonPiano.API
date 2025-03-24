using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class PianoSurveyQuestionConfiguration : IEntityTypeConfiguration<PianoSurveyQuestion>
{
    public void Configure(EntityTypeBuilder<PianoSurveyQuestion> builder)
    {
        builder.HasKey(x => new { x.SurveyId, x.QuestionId });
        
        builder.Property(x => x.IsRequired)
            .HasDefaultValue(false);

        builder.HasOne(x => x.Survey)
            .WithMany(s => s.PianoSurveyQuestions)
            .HasForeignKey(x => x.SurveyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Question)
            .WithMany(q => q.PianoSurveyQuestions)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}