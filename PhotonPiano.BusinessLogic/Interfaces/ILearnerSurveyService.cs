using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ILearnerSurveyService
{
    Task<PagedResult<LearnerSurveyModel>> GetPagedSurveys(QueryPagedSurveysModel queryModel);
}