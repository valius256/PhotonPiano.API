using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IPianoSurveyService
{
    Task<PagedResult<PianoSurveyModel>> GetSurveys(QueryPagedSurveysModel query, AccountModel currentAccount);
    Task<PianoSurveyDetailsModel> GetSurveyDetails(Guid id, AccountModel currentAccount);
    Task<PianoSurveyDetailsModel> CreatePianoSurvey(CreatePianoSurveyModel createModel, AccountModel currentAccount);
    Task UpdatePianoSurvey(Guid id, UpdatePianoSurveyModel updateModel, AccountModel currentAccount);
    
    Task DeletePianoSurvey(Guid id, AccountModel currentAccount);

    Task<PianoSurveyDetailsModel> GetEntranceSurvey();

    Task<AuthModel> SendEntranceSurveyAnswers(SendEntranceSurveyAnswersModel model);
}