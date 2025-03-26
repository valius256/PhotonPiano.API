using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Services;

public class LearnerSurveyService : ILearnerSurveyService
{
    private readonly IUnitOfWork _unitOfWork;

    public LearnerSurveyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<LearnerSurveyModel>> GetPagedSurveys(QueryPagedSurveysModel queryModel)
    {
        var (page, size, column, desc) = queryModel;

        return await _unitOfWork.LearnerSurveyRepository.GetPaginatedWithProjectionAsync<LearnerSurveyModel>(page, size,
            column, desc);
    }

}