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
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class SurveyQuestionService : ISurveyQuestionService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    public SurveyQuestionService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<PagedResult<SurveyQuestionModel>> GetPagedSurveyQuestions(
        QueryPagedSurveyQuestionsModel queryModel)
    {
        var (page, size, column, desc, keyword) = queryModel;

        return await _unitOfWork.SurveyQuestionRepository.GetPaginatedWithProjectionAsync<SurveyQuestionModel>(
            page, size, column, desc, expressions:
            [
                q => string.IsNullOrEmpty(keyword) || q.QuestionContent.ToLower().Contains(keyword.ToLower())
            ]);
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

    public async Task<PagedResult<LearnerAnswerWithLearnerModel>> GetQuestionAnswers(Guid id,
        QueryPagedAnswersModel queryModel,
        AccountModel currentAccount)
    {
        if (!await _unitOfWork.SurveyQuestionRepository.AnyAsync(q => q.Id == id))
        {
            throw new NotFoundException("Survey question not found");
        }

        var (page, size, column, desc, keyword) = queryModel;

        var answers = await _unitOfWork.LearnerAnswerRepository
            .GetPaginatedWithProjectionAsync<LearnerAnswerWithLearnerModel>(
                page, size, column, desc, expressions:
                [
                    a => a.SurveyQuestionId == id,
                    q => currentAccount.Role == Role.Staff ||
                         q.LearnerSurvey.LearnerId == currentAccount.AccountFirebaseId,
                ]);

        return answers;
    }

    public async Task<SurveyQuestionModel> CreateSurveyQuestion(CreateSurveyQuestionModel createModel,
        AccountModel currentAccount)
    {
        if (createModel is
            {
                Type: QuestionType.MultipleChoice or QuestionType.LikertScale or QuestionType.SingleChoice,
            })
        {
            if (createModel.Options.Count == 0)
            {
                throw new BadRequestException("Options can't be empty for this question type");
            }

            var surveyConfigs = await _serviceFactory.SystemConfigService.GetAllSurveyConfigs();
            
            var instrumentNameConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.InstrumentName);
            var instrumentFrequencyConfig =
                surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.InstrumentFrequencyInResponse);

            if (instrumentNameConfig is not null && instrumentFrequencyConfig is not null)
            {
                string instrumentName = instrumentNameConfig.ConfigValue ?? string.Empty;

                int frequency = Convert.ToInt32(instrumentFrequencyConfig.ConfigValue ?? "0");

                if (createModel.Options.Count(o => o.ToLower().Contains(instrumentName.ToLower())) < frequency)
                {
                    throw new BadRequestException(
                        $"Options in question must contain the instrument name {instrumentName} at least {frequency} times");
                }
            }
        }

        if (createModel.SurveyId.HasValue)
        {
            var survey =
                await _unitOfWork.PianoSurveyRepository.GetPianoSurveyWithQuestionsAsync(createModel.SurveyId.Value);

            if (survey is null)
            {
                throw new NotFoundException("Survey not found");
            }

            if (survey.PianoSurveyQuestions.Any(q => q.OrderIndex == createModel.OrderIndex))
            {
                throw new ConflictException("This order index is already in use");
            }

            if (createModel.MinAge < survey.MinAge || createModel.MaxAge > survey.MaxAge)
            {
                throw new BadRequestException("This question has invalid age for this survey");
            }
        }

        var question = createModel.Adapt<SurveyQuestion>();

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            question.CreatedById = currentAccount.AccountFirebaseId;

            await _unitOfWork.SurveyQuestionRepository.AddAsync(question);

            await _unitOfWork.SaveChangesAsync();


            if (createModel.SurveyId.HasValue && createModel.OrderIndex.HasValue)
            {
                //Add to PianoSurveyQuestion table
                await _unitOfWork.PianoSurveyQuestionRepository.AddAsync(new PianoSurveyQuestion
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    SurveyId = createModel.SurveyId.Value,
                    OrderIndex = createModel.OrderIndex.Value,
                    IsRequired = createModel.IsRequired
                });
            }
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