using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SurveyQuestionService : ISurveyQuestionService
{
    private readonly IUnitOfWork _unitOfWork;

    public SurveyQuestionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SurveyQuestionModel>> GetPagedSurveyQuestions(
        QueryPagedSurveyQuestionsModel queryModel)
    {
        var (page, size, column, desc) = queryModel;

        return await _unitOfWork.SurveyQuestionRepository.GetPaginatedWithProjectionAsync<SurveyQuestionModel>(
            page, size, column, desc);
    }

    public async Task<SurveyQuestionDetailsModel> GetSurveyQuestionDetails(Guid id)
    {
        var surveyQuestion =
            await _unitOfWork.SurveyQuestionRepository.FindSingleProjectedAsync<SurveyQuestionDetailsModel>(
                q => q.Id == id, hasTrackings: false,
                option: TrackingOption.IdentityResolution);

        if (surveyQuestion is null)
        {
            throw new NotFoundException("Survey question not found");
        }

        return surveyQuestion;
    }

    public async Task<SurveyQuestionModel> CreateSurveyQuestion(CreateSurveyQuestionModel createModel,
        AccountModel currentAccount)
    {
        if (createModel is
            {
                Type: QuestionType.MultipleChoice or QuestionType.LikertScale or QuestionType.SingleChoice,
                Options.Count: 0
            })
        {
            throw new BadRequestException("Options can't be empty for this question type");
        }
        
        var survey =
            await _unitOfWork.PianoSurveyRepository.GetPianoSurveyWithQuestionsAsync(createModel.SurveyId);

        if (survey is null)
        {
            throw new NotFoundException("Survey not found");
        }

        if (survey.Questions.Any(q => q.OrderIndex == createModel.OrderIndex))
        {
            throw new ConflictException("This order index is already in use");
        }

        var question = createModel.Adapt<SurveyQuestion>();
        
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            question.CreatedById = currentAccount.AccountFirebaseId;

            await _unitOfWork.SurveyQuestionRepository.AddAsync(question);

            await _unitOfWork.SaveChangesAsync();
            
            survey.Questions.Add(question);
        });
        
        return question.Adapt<SurveyQuestionModel>();
    }

    public async Task UpdateSurveyQuestion(Guid id, UpdateSurveyQuestionModel updateModel, AccountModel currentAccount)
    {
        var surveyQuestion = await _unitOfWork.SurveyQuestionRepository.FindSingleAsync(s => s.Id == id);

        if (surveyQuestion is null)
        {
            throw new NotFoundException("Survey question not found.");
        }

        updateModel.Adapt(surveyQuestion);

        surveyQuestion.UpdatedById = currentAccount.AccountFirebaseId;
        surveyQuestion.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSurveyQuestion(Guid id, AccountModel currentAccount)
    {
        var surveyQuestion = await _unitOfWork.SurveyQuestionRepository.FindSingleAsync(s => s.Id == id);

        if (surveyQuestion is null)
        {
            throw new NotFoundException("Survey question not found.");
        }

        surveyQuestion.RecordStatus = RecordStatus.IsDeleted;
        surveyQuestion.DeletedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
    }
}