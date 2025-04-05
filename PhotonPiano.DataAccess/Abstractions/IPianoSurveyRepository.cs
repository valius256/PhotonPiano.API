using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Abstractions;

public interface IPianoSurveyRepository : IGenericRepository<PianoSurvey>
{
    Task<PianoSurvey?> GetPianoSurveyWithQuestionsAsync(Guid id);
}