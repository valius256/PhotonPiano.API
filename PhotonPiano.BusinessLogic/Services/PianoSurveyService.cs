using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class PianoSurveyService : IPianoSurveyService
{
    private readonly IUnitOfWork _unitOfWork;

    public PianoSurveyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<PianoSurveyModel>> GetSurveys(QueryPagedSurveysModel query,
        AccountModel currentAccount)
    {
        var (page, size, column, desc) = query;

        return await _unitOfWork.PianoSurveyRepository.GetPaginatedWithProjectionAsync<PianoSurveyModel>(page, size,
            column, desc, expressions:
            [
                s => string.IsNullOrEmpty(query.Keyword) || s.Name.ToLower().Contains(query.Keyword.ToLower())
            ]);
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

        return survey;
    }

    public async Task<PianoSurveyModel> CreatePianoSurvey(CreatePianoSurveyModel createModel,
        AccountModel currentAccount)
    {
        var survey = createModel.Adapt<PianoSurvey>();

        survey.CreatedById = currentAccount.AccountFirebaseId;

        await _unitOfWork.PianoSurveyRepository.AddAsync(survey);

        await _unitOfWork.SaveChangesAsync();

        return survey.Adapt<PianoSurveyModel>();
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

        await _unitOfWork.SaveChangesAsync();
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