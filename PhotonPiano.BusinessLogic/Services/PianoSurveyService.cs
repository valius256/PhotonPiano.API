using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class PianoSurveyService : IPianoSurveyService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    public PianoSurveyService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    private async Task<Guid?> GetEntranceSurveyConfigId()
    {
        var entranceSurveyConfig = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.EntranceSurvey);

        return entranceSurveyConfig.ConfigValue is null ? null : Guid.Parse(entranceSurveyConfig.ConfigValue);
    }

    public async Task<PagedResult<PianoSurveyModel>> GetSurveys(QueryPagedSurveysModel query,
        AccountModel currentAccount)
    {
        var (page, size, column, desc) = query;

        var entranceSurveyId = await GetEntranceSurveyConfigId();

        var pagedResult = await _unitOfWork.PianoSurveyRepository.GetPaginatedWithProjectionAsync<PianoSurveyModel>(
            page, size,
            column, desc, expressions:
            [
                s => string.IsNullOrEmpty(query.Keyword) || s.Name.ToLower().Contains(query.Keyword.ToLower())
            ]);

        if (entranceSurveyId is not null)
        {
            foreach (var survey in pagedResult.Items.Where(survey => survey.Id == entranceSurveyId.Value))
            {
                survey.IsEntranceSurvey = true;
                break;
            }
        }

        return pagedResult;
    }

    public async Task<PianoSurveyDetailsModel> GetSurveyDetails(Guid id, AccountModel currentAccount)
    {
        var survey = await _unitOfWork.PianoSurveyRepository.FindSingleProjectedAsync<PianoSurveyDetailsModel>(
            s => s.Id == id,
            hasTrackings: false);

        if (survey is null)
        {
            throw new NotFoundException("Survey not found");
        }

        if (currentAccount.Role != Role.Staff &&
            survey.LearnerSurveys.All(ls => ls.LearnerId != currentAccount.AccountFirebaseId))
        {
            throw new ForbiddenMethodException("You do not have access to this survey");
        }

        var entranceSurveyId = await GetEntranceSurveyConfigId();

        if (entranceSurveyId is not null)
        {
            survey.IsEntranceSurvey = true;
        }

        return survey;
    }

    public async Task<PianoSurveyDetailsModel> CreatePianoSurvey(CreatePianoSurveyModel createModel,
        AccountModel currentAccount)
    {
        var survey = createModel.Adapt<PianoSurvey>();

        survey.Id = Guid.NewGuid();
        survey.CreatedById = currentAccount.AccountFirebaseId;

        List<SurveyQuestion> surveyQuestionsToAdd = [];
        List<SurveyQuestion> dbQuestions = [];
        List<PianoSurveyQuestion> pianoSurveyQuestions = [];

        if (createModel.CreateQuestionRequests.Count > 0)
        {
            var dbQuestionIds = new List<Guid>();

            int index = 0;

            foreach (var request in createModel.CreateQuestionRequests)
            {
                if (request.Id.HasValue)
                {
                    dbQuestionIds.Add(request.Id.Value);
                    pianoSurveyQuestions.Add(new PianoSurveyQuestion
                    {
                        QuestionId = request.Id.Value,
                        SurveyId = survey.Id,
                        OrderIndex = index,
                        IsRequired = request.IsRequired,
                    });
                }
                else
                {
                    var newQuestion = request.Adapt<SurveyQuestion>();
                    newQuestion.Id = Guid.NewGuid();
                    newQuestion.CreatedById = currentAccount.AccountFirebaseId;
                    newQuestion.MinAge = survey.MinAge;
                    newQuestion.MaxAge = survey.MaxAge;
                    surveyQuestionsToAdd.Add(newQuestion);
                    pianoSurveyQuestions.Add(new PianoSurveyQuestion
                    {
                        QuestionId = newQuestion.Id,
                        SurveyId = survey.Id,
                        OrderIndex = index,
                        IsRequired = request.IsRequired,
                    });
                }

                ++index;
            }

            dbQuestions = await _unitOfWork.SurveyQuestionRepository.FindAsync(q => dbQuestionIds.Contains(q.Id),
                hasTrackings: false);

            if (dbQuestionIds.Count != dbQuestions.Count)
            {
                throw new BadRequestException("Some of questions are not found");
            }
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.PianoSurveyRepository.AddAsync(survey);

            if (createModel.CreateQuestionRequests.Count > 0)
            {
                if (surveyQuestionsToAdd.Count > 0)
                {
                    await _unitOfWork.SurveyQuestionRepository.AddRangeAsync(surveyQuestionsToAdd);
                }

                if (pianoSurveyQuestions.Count > 0)
                {
                    await _unitOfWork.PianoSurveyQuestionRepository.AddRangeAsync(pianoSurveyQuestions);
                }
            }
        });

        return await GetSurveyDetails(survey.Id, currentAccount);
    }

    public async Task UpdatePianoSurvey(Guid id, UpdatePianoSurveyModel updateModel, AccountModel currentAccount)
    {
        var survey = await _unitOfWork.PianoSurveyRepository.FindSingleAsync(s => s.Id == id);

        if (survey is null)
        {
            throw new NotFoundException("Survey not found");
        }

        updateModel.Adapt(survey);
        survey.UpdatedAt = DateTime.UtcNow.AddHours(7);
        survey.UpdatedById = currentAccount.AccountFirebaseId;

        if (updateModel.RecordStatus is RecordStatus.IsDeleted)
        {
            survey.DeletedAt = DateTime.UtcNow.AddHours(7);
        }


        if (updateModel.IsEntranceSurvey.HasValue)
        {
            var entranceSurveyConfig =
                await _unitOfWork.SystemConfigRepository.FindSingleAsync(
                    c => c.ConfigName == ConfigNames.EntranceSurvey && c.RecordStatus == RecordStatus.IsActive);

            if (entranceSurveyConfig is not null)
            {
                entranceSurveyConfig.ConfigValue = updateModel.IsEntranceSurvey == true
                    ? id.ToString()
                    : entranceSurveyConfig.ConfigValue;
            }
        }

        List<SurveyQuestion> surveyQuestionsToAdd = [];
        List<SurveyQuestion> dbQuestions = [];
        List<PianoSurveyQuestion> pianoSurveyQuestions = [];

        if (updateModel.Questions.Count > 0)
        {
            var dbQuestionIds = new List<Guid>();
            int index = 0;

            foreach (var request in updateModel.Questions)
            {
                if (request.Id.HasValue)
                {
                    dbQuestionIds.Add(request.Id.Value);
                    pianoSurveyQuestions.Add(new PianoSurveyQuestion
                    {
                        QuestionId = request.Id.Value,
                        SurveyId = survey.Id,
                        OrderIndex = index,
                        IsRequired = request.IsRequired,
                    });
                }
                else
                {
                    var newQuestion = request.Adapt<SurveyQuestion>();
                    newQuestion.Id = Guid.NewGuid();
                    newQuestion.CreatedById = currentAccount.AccountFirebaseId;
                    newQuestion.MinAge = survey.MinAge;
                    newQuestion.MaxAge = survey.MaxAge;
                    surveyQuestionsToAdd.Add(newQuestion);
                    pianoSurveyQuestions.Add(new PianoSurveyQuestion
                    {
                        QuestionId = newQuestion.Id,
                        SurveyId = survey.Id,
                        OrderIndex = index,
                        IsRequired = request.IsRequired,
                    });
                }

                ++index;
            }

            dbQuestions =
                await _unitOfWork.SurveyQuestionRepository.FindAsync(q => dbQuestionIds.Contains(q.Id),
                    hasTrackings: false);

            if (dbQuestionIds.Count != dbQuestions.Count)
            {
                throw new BadRequestException("Some of questions are not found");
            }
        }


        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.Questions.Count > 0)
            {
                // await _unitOfWork.PianoSurveyRepository.UpdateAsync(survey);
                // await _unitOfWork.SaveChangesAsync();
                // // survey.PianoSurveyQuestions.Clear();
                //
                //
                // _unitOfWork.PianoSurveyRepository.Detach(survey);
                //
                if (surveyQuestionsToAdd.Count > 0)
                {
                    await _unitOfWork.SurveyQuestionRepository.AddRangeAsync(surveyQuestionsToAdd);
                }

                if (pianoSurveyQuestions.Count > 0)
                {
                    await _unitOfWork.PianoSurveyQuestionRepository.ExecuteDeleteAsync(x => x.SurveyId == survey.Id);
                    // await _unitOfWork.SaveChangesAsync();
                    
                    await _unitOfWork.PianoSurveyQuestionRepository.AddRangeAsync(pianoSurveyQuestions);
                }
            }
        });
    }

    public async Task<PianoSurveyDetailsModel> GetEntranceSurvey()
    {
        var entranceSurveyConfig = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.EntranceSurvey);

        if (entranceSurveyConfig?.ConfigValue is null)
        {
            throw new NotFoundException("Entrance survey not found");
        }

        Guid surveyId = Guid.Parse(entranceSurveyConfig.ConfigValue);

        var survey =
            await _unitOfWork.PianoSurveyRepository.FindSingleProjectedAsync<PianoSurveyDetailsModel>(
                s => s.Id == surveyId, hasTrackings: false);

        if (survey is null)
        {
            throw new NotFoundException("Survey not found");
        }

        return survey;
    }

    public async Task<PianoSurveyDetailsModel> CreatePianoSurveyAnswers(Guid surveyId,
        CreateSurveyAnswersModel createModel, AccountModel currentAccount)
    {
        if (!await _unitOfWork.PianoSurveyRepository.AnyAsync(s => s.Id == surveyId))
        {
            throw new NotFoundException("Survey not found");
        }

        var learnerSurvey = new LearnerSurvey
        {
            Id = Guid.NewGuid(),
            LearnerId = currentAccount.AccountFirebaseId,
            PianoSurveyId = surveyId,
            LearnerEmail = currentAccount.Email,
        };

        var questionIds = createModel.CreateAnswerRequests.Select(r => r.SurveyQuestionId);

        var questions =
            await _unitOfWork.SurveyQuestionRepository.FindAsync(
                q => questionIds.Contains(q.Id), hasTrackings: false);

        if (questions.Count != questionIds.Count())
        {
            throw new BadRequestException("Some of the questions are not found");
        }

        var learnerAnswers = new List<LearnerAnswer>();

        foreach (var request in createModel.CreateAnswerRequests)
        {
            var answeringQuestion = questions.SingleOrDefault(q => q.Id == request.SurveyQuestionId);

            if (answeringQuestion is null)
            {
                throw new BadRequestException("Survey question not found");
            }

            learnerAnswers.Add(new LearnerAnswer
            {
                Id = Guid.NewGuid(),
                SurveyQuestionId = answeringQuestion.Id,
                LearnerSurveyId = learnerSurvey.Id,
                Answers = request.Answers,
            });
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.LearnerSurveyRepository.AddAsync(learnerSurvey);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.LearnerAnswerRepository.AddRangeAsync(learnerAnswers);
        });

        return await GetSurveyDetails(surveyId, currentAccount);
    }
}