using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class PianoQuestionService : IPianoQuestionService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;
    
    private readonly string questionsCacheKey = "surveyQuestions";
    
    public PianoQuestionService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<PagedResult<PianoQuestionModel>> GetPagedSurveyQuestions(
        QueryPagedPianoQuestionsModel queryModel)
    {
        var (page, size, column, desc) = queryModel;

        return await _unitOfWork.PianoQuestionRepository.GetPaginatedWithProjectionAsync<PianoQuestionModel>(
            page, size, column, desc);
    }

    public async Task<List<PianoQuestionModel>> GetAllSurveyQuestions()
    {
        return await _unitOfWork.PianoQuestionRepository.FindProjectedAsync<PianoQuestionModel>(q => true,
            hasTrackings: false);
    }

    public async Task<List<PianoQuestionModel>> GetCachedAllSurveyQuestions()
    {
        var cacheValue = await _serviceFactory.RedisCacheService.GetAsync<List<PianoQuestionModel>>(questionsCacheKey);

        Console.WriteLine($"Questions cache: {cacheValue}");

        if (cacheValue is not null)
        {
            return cacheValue;
        }

        var questions = await GetAllSurveyQuestions();
        
        await _serviceFactory.RedisCacheService.SaveAsync(questionsCacheKey, questions, TimeSpan.FromDays(7));

        return questions;
    }

    public async Task<PianoQuestionDetailsModel> GetSurveyQuestionDetails(Guid id)
    {
        var surveyQuestion =
            await _unitOfWork.PianoQuestionRepository.FindSingleProjectedAsync<PianoQuestionDetailsModel>(
                q => q.Id == id, hasTrackings: false,
                option: TrackingOption.IdentityResolution);

        if (surveyQuestion is null)
        {
            throw new NotFoundException("Survey question not found");
        }

        return surveyQuestion;
    }

    public async Task<PianoQuestionModel> CreatePianoQuestion(CreatePianoQuestionModel createModel,
        AccountModel currentAccount)
    {
        var surveyQuestion = createModel.Adapt<PianoQuestion>();
        surveyQuestion.CreatedById = currentAccount.AccountFirebaseId;

        await _unitOfWork.PianoQuestionRepository.AddAsync(surveyQuestion);

        await _unitOfWork.SaveChangesAsync();
        
        await _serviceFactory.RedisCacheService.DeleteAsync(questionsCacheKey);

        return surveyQuestion.Adapt<PianoQuestionModel>();
    }

    public async Task UpdatePianoQuestion(Guid id, UpdatePianoQuestionModel updateModel, AccountModel currentAccount)
    {
        var surveyQuestion = await _unitOfWork.PianoQuestionRepository.FindSingleAsync(s => s.Id == id);

        if (surveyQuestion is null)
        {
            throw new NotFoundException("Survey question not found.");
        }

        updateModel.Adapt(surveyQuestion);

        surveyQuestion.UpdatedById = currentAccount.AccountFirebaseId;
        surveyQuestion.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
        
        await _serviceFactory.RedisCacheService.DeleteAsync(questionsCacheKey);
    }

    public async Task DeletePianoQuestion(Guid id, AccountModel currentAccount)
    {
        var surveyQuestion = await _unitOfWork.PianoQuestionRepository.FindSingleAsync(s => s.Id == id);

        if (surveyQuestion is null)
        {
            throw new NotFoundException("Survey question not found.");
        }

        surveyQuestion.RecordStatus = RecordStatus.IsDeleted;
        surveyQuestion.DeletedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
        
        await _serviceFactory.RedisCacheService.DeleteAsync(questionsCacheKey);
    }
}