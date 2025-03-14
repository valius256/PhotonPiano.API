using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IPianoQuestionService
{
    Task<PagedResult<PianoQuestionModel>> GetPagedSurveyQuestions(QueryPagedPianoQuestionsModel queryModel);
    
    Task<List<PianoQuestionModel>> GetAllSurveyQuestions();
    
    Task<List<PianoQuestionModel>> GetCachedAllSurveyQuestions();
    
    Task<PianoQuestionDetailsModel> GetSurveyQuestionDetails(Guid id);

    Task<PianoQuestionModel> CreatePianoQuestion(CreatePianoQuestionModel createModel, AccountModel currentAccount);
    
    Task UpdatePianoQuestion(Guid id, UpdatePianoQuestionModel updateModel, AccountModel currentAccount);
    
    Task DeletePianoQuestion(Guid id, AccountModel currentAccount);
}