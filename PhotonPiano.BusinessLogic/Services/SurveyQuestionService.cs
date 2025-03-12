using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SurveyQuestionService : ISurveryQuestionService
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
        var surveyQuestion = createModel.Adapt<SurveyQuestion>();
        surveyQuestion.CreatedById = currentAccount.AccountFirebaseId;

        await _unitOfWork.SurveyQuestionRepository.AddAsync(surveyQuestion);

        await _unitOfWork.SaveChangesAsync();

        return surveyQuestion.Adapt<SurveyQuestionModel>();
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