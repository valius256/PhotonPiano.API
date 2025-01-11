using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class CriteriaService : ICriteriaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CriteriaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<PagedResult<CriteriaDetailModel>> GetPagedCriteria(QueryCriteriaModel query)
    {
        var (page, pageSize, sortColumn, orderByDesc, keyword)
            = query;
        string likeKeyword = query.GetLikeKeyword();
        var result = await  _unitOfWork.CriteriaRepository.GetPaginatedWithProjectionAsync<CriteriaDetailModel>(page, pageSize,
            sortColumn, orderByDesc,
            expressions:
            [
                c => !string.IsNullOrEmpty(keyword) || 
                     EF.Functions.ILike(EF.Functions.Unaccent(c.Name), likeKeyword) || 
                     EF.Functions.ILike(EF.Functions.Unaccent(c.Description?? string.Empty), likeKeyword)
            ]);
        return result;
    }

    public async Task<CriteriaDetailModel> GetCriteriaDetailById(Guid id)
    {
        var result = await _unitOfWork.CriteriaRepository
            .FindSingleProjectedAsync<CriteriaDetailModel>(e => e.Id == id, hasTrackings: false);
        if (result is null)
        {
            throw new NotFoundException("Criteria not found.");
        }
        return result;
    }
}