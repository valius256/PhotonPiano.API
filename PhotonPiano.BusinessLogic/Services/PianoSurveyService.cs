using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
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
        var surveyConfigs = await _serviceFactory.SystemConfigService.GetAllSurveyConfigs();

        var instrumentNameConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.InstrumentName);
        var instrumentFrequencyConfig =
            surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.InstrumentFrequencyInResponse);

        var maxQuestionsConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.MaxQuestionsPerSurvey);
        var minQuestionsConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.MinQuestionsPerSurvey);

        if (minQuestionsConfig is not null && maxQuestionsConfig is not null)
        {
            int minQuestions = Convert.ToInt32(minQuestionsConfig.ConfigValue);

            int maxQuestions = Convert.ToInt32(maxQuestionsConfig.ConfigValue);

            if (createModel.CreateQuestionRequests.Count < minQuestions ||
                createModel.CreateQuestionRequests.Count > maxQuestions)
            {
                throw new BadRequestException(
                    $"Number of questions in survey must between {minQuestions} and {maxQuestions}");
            }
        }

        if (instrumentNameConfig is not null && instrumentFrequencyConfig is not null)
        {
            string instrumentName = instrumentNameConfig.ConfigValue ?? string.Empty;

            if (!createModel.Name.ToLower().Contains(instrumentName.ToLower()))
            {
                throw new BadRequestException(
                    $"The survey name doesn't contains the instrument name: {instrumentName}");
            }

            int frequency = Convert.ToInt32(instrumentFrequencyConfig.ConfigValue ?? "0");

            var allOptions = createModel.CreateQuestionRequests.SelectMany(r => r.Options);

            if (allOptions.Count(o => o.ToLower().Contains(instrumentName.ToLower())) < frequency)
            {
                throw new BadRequestException(
                    $"Options in question must contain the instrument name {instrumentName} at least {frequency} times");
            }

            // if (createModel.CreateQuestionRequests.Any(question =>
            //         question.Options.Count(o => o.ToLower().Contains(instrumentName.ToLower())) < frequency))
            // {
            //     throw new BadRequestException(
            //         $"Options in question must contain the instrument name {instrumentName} at least {frequency} times");
            // }
        }

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
                        Id = Guid.NewGuid(),
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
                        Id = Guid.NewGuid(),
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
        var survey = await _unitOfWork.PianoSurveyRepository.GetPianoSurveyWithQuestionsAsync(id);

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

        var surveyConfigs = await _serviceFactory.SystemConfigService.GetAllSurveyConfigs();

        var instrumentNameConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.InstrumentName);
        string instrumentName =
            instrumentNameConfig is not null ? instrumentNameConfig.ConfigValue ?? "" : string.Empty;
        if (!string.IsNullOrEmpty(updateModel.Name))
        {
            if (!updateModel.Name.ToLower().Contains(instrumentName.ToLower()))
            {
                throw new BadRequestException($"Survey name must contains the instrument name {instrumentName}");
            }
        }

        var maxQuestionsConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.MaxQuestionsPerSurvey);
        var minQuestionsConfig = surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.MinQuestionsPerSurvey);

        if (minQuestionsConfig is not null && maxQuestionsConfig is not null)
        {
            int minQuestions = Convert.ToInt32(minQuestionsConfig.ConfigValue);

            int maxQuestions = Convert.ToInt32(maxQuestionsConfig.ConfigValue);

            if (updateModel.Questions.Count < minQuestions ||
                updateModel.Questions.Count > maxQuestions)
            {
                throw new BadRequestException(
                    $"Number of questions in survey must between {minQuestions} and {maxQuestions}");
            }
        }

        var instrumentFrequencyConfig =
            surveyConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.InstrumentFrequencyInResponse);

        int frequency = instrumentFrequencyConfig is not null
            ? Convert.ToInt32(instrumentFrequencyConfig.ConfigValue ?? "0")
            : 0;

        List<SurveyQuestion> surveyQuestionsToAdd = [];
        List<SurveyQuestion> dbQuestions = [];
        List<PianoSurveyQuestion> pianoSurveyQuestions = [];

        if (updateModel.Questions.Count > 0)
        {
            var dbQuestionIds = new List<Guid>();
            int index = 0;

            var allOptions = updateModel.Questions.SelectMany(q => q.Options);
            
            if (allOptions.Count(o => o.ToLower().Contains(instrumentName.ToLower())) < frequency)
            {
                throw new BadRequestException(
                    $"Options in question must contain the instrument name {instrumentName} at least {frequency} times");
            }

            foreach (var request in updateModel.Questions)
            {
                if (request.Id.HasValue)
                {
                    dbQuestionIds.Add(request.Id.Value);
                    pianoSurveyQuestions.Add(new PianoSurveyQuestion
                    {
                        Id = Guid.NewGuid(),
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
                        Id = Guid.NewGuid(),
                        QuestionId = newQuestion.Id,
                        SurveyId = survey.Id,
                        OrderIndex = index,
                        IsRequired = request.IsRequired,
                    });
                }

                ++index;
            }

            dbQuestions =
                await _unitOfWork.SurveyQuestionRepository.FindAsync(q => dbQuestionIds.Contains(q.Id));

            if (dbQuestionIds.Count != dbQuestions.Count)
            {
                throw new BadRequestException("Some of questions are not found");
            }

            foreach (var question in dbQuestions)
            {
                var updatedQuestion = updateModel.Questions.FirstOrDefault(q => q.Id == question.Id);

                if (updatedQuestion is null)
                {
                    continue;
                }
                
                question.QuestionContent = updatedQuestion.QuestionContent;
                question.Options = updatedQuestion.Options;
                question.AllowOtherAnswer = updatedQuestion.AllowOtherAnswer;
            }
            
        }

        foreach (var question in survey.PianoSurveyQuestions)
        {
            _unitOfWork.PianoSurveyQuestionRepository.Detach(question);
        }

        await _unitOfWork.PianoSurveyQuestionRepository.DeleteRangeAsync(survey.PianoSurveyQuestions);
        await _unitOfWork.SaveChangesAsync();

        survey.PianoSurveyQuestions.Clear();

        foreach (var question in pianoSurveyQuestions)
        {
            survey.PianoSurveyQuestions.Add(question);
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.Questions.Count > 0)
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
    }

    public async Task DeletePianoSurvey(Guid id, AccountModel currentAccount)
    {
        var survey = await _unitOfWork.PianoSurveyRepository.FindSingleAsync(s => s.Id == id);

        if (survey is null)
        {
            throw new NotFoundException("Survey not found");
        }

        survey.RecordStatus = RecordStatus.IsDeleted;
        survey.DeletedAt = DateTime.UtcNow.AddHours(7);
        survey.UpdatedById = currentAccount.AccountFirebaseId;
        survey.DeletedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
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

    public async Task SendEntranceSurveyAnswers(SendEntranceSurveyAnswersModel model)
    {
        var (email, password, fullName, phone, answers) = model;

        var entranceSurveyConfig = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.EntranceSurvey);

        if (entranceSurveyConfig?.ConfigValue is null)
        {
            throw new NotFoundException("Entrance survey not found");
        }

        var surveyId = Guid.Parse(entranceSurveyConfig.ConfigValue);

        var survey =
            await _unitOfWork.PianoSurveyRepository.FindSingleProjectedAsync<PianoSurveyWithQuestionsModel>(
                s => s.Id == surveyId, hasTrackings: false);

        if (survey is null)
        {
            throw new NotFoundException("Entrance survey not found");
        }

        var learnerAnswers = new List<LearnerAnswer>();

        foreach (var question in survey.PianoSurveyQuestions)
        {
            if (question.IsRequired && answers.All(a => a.SurveyQuestionId != question.QuestionId))
            {
                throw new BadRequestException("Some required questions are not answered");
            }

            var questionAnswers = answers.FirstOrDefault(a => a.SurveyQuestionId == question.QuestionId);

            learnerAnswers.Add(new LearnerAnswer
            {
                Id = Guid.NewGuid(),
                SurveyQuestionId = question.QuestionId,
                Answers = questionAnswers is not null ? questionAnswers.Answers : [],
            });
        }

        var account = await _serviceFactory.AuthService.SignUp(new SignUpModel
        {
            Email = email,
            Password = password,
            FullName = fullName,
            Phone = phone
        });

        var learnerSurvey = new LearnerSurvey
        {
            Id = Guid.NewGuid(),
            LearnerId = account.AccountFirebaseId,
            PianoSurveyId = surveyId,
            LearnerEmail = account.Email,
            LearnerAnswers = learnerAnswers,
        };

        foreach (var learnerAnswer in learnerAnswers)
        {
            learnerAnswer.LearnerSurveyId = learnerSurvey.Id;
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.LearnerSurveyRepository.AddAsync(learnerSurvey);

            await _unitOfWork.LearnerAnswerRepository.AddRangeAsync(learnerAnswers);
        });
    }
}