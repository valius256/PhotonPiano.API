using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISurveyQuestionService
{
    Task<PagedResult<SurveyQuestionModel>> GetPagedSurveyQuestions(QueryPagedSurveyQuestionsModel queryModel);

    Task<SurveyQuestionDetailsModel> GetSurveyQuestionDetails(Guid id);

    Task<PagedResult<LearnerAnswerWithLearnerModel>> GetQuestionAnswers(Guid id, QueryPagedAnswersModel queryModel,
        AccountModel currentAccount);

    Task<SurveyQuestionModel> CreateSurveyQuestion(CreateSurveyQuestionModel createModel, AccountModel currentAccount);

    Task UpdateSurveyQuestion(Guid id, UpdateSurveyQuestionModel updateModel, AccountModel currentAccount);

    Task DeleteSurveyQuestion(Guid id, AccountModel currentAccount);
}