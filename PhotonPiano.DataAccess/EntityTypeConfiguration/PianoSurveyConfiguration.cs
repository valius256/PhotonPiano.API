using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.EntityTypeConfiguration;

public class PianoSurveyConfiguration : IEntityTypeConfiguration<PianoSurvey>
{
    public void Configure(EntityTypeBuilder<PianoSurvey> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasQueryFilter(x => x.RecordStatus != RecordStatus.IsDeleted);

        builder.HasMany(s => s.SurveyQuestions)
            .WithMany(q => q.PianoSurveys)
            .UsingEntity(
                "SurveyQuestion",
                l => l.HasOne(typeof(SurveyQuestion))
                    .WithMany()
                    .HasForeignKey("QuestionId")
                    .HasPrincipalKey(nameof(SurveyQuestion.Id))
                    .OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(typeof(PianoSurvey))
                    .WithMany()
                    .HasForeignKey("SurveyId")
                    .HasPrincipalKey(nameof(PianoSurvey.Id))
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasKey("QuestionId", "SurveyId")
            );
        
        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedPianoSurveys)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.UpdatedBy)
            .WithMany(x => x.UpdatedPianoSurveys)
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}