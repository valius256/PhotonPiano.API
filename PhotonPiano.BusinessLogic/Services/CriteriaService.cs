using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class CriteriaService : ICriteriaService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public CriteriaService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }


    public async Task<List<MinimalCriteriaModel>> GetMinimalCriterias(QueryMinimalCriteriasModel queryModel)
    {
        return await _unitOfWork.CriteriaRepository.FindProjectedAsync<MinimalCriteriaModel>(
            c => c.For == queryModel.CriteriaFor,
            hasTrackings: false);
    }

    public async Task<PagedResult<CriteriaDetailModel>> GetPagedCriteria(QueryCriteriaModel query)
    {
        // var cacheCriteria =
        //     await _serviceFactory.RedisCacheService.GetAsync<PagedResult<CriteriaDetailModel>>("criteria");
        // if (cacheCriteria is not null) return cacheCriteria;

        var (page, pageSize, sortColumn, orderByDesc, keyword)
            = query;
        var likeKeyword = query.GetLikeKeyword();
        var result = await _unitOfWork.CriteriaRepository.GetPaginatedWithProjectionAsync<CriteriaDetailModel>(page,
            pageSize,
            sortColumn, orderByDesc,
            expressions:
            [
                c => !string.IsNullOrEmpty(keyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(c.Name), likeKeyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(c.Description ?? string.Empty), likeKeyword)
            ]);
        var allCriterial = await _unitOfWork.CriteriaRepository.GetAllAsync();
        // await _serviceFactory.RedisCacheService.SaveAsync("criteria", allCriterial, TimeSpan.FromHours(10));
        return result;
    }

    public async Task<CriteriaDetailModel> GetCriteriaDetailById(Guid id)
    {
        var result = await _unitOfWork.CriteriaRepository
            .FindSingleProjectedAsync<CriteriaDetailModel>(e => e.Id == id, false);
        if (result is null) throw new NotFoundException("Criteria not found.");
        return result;
    }
    public async Task<List<CriteriaGradeModel>> GetAllCriteriaDetails(Guid classId,
        CriteriaFor criteriaType = CriteriaFor.Class)
    {
        var query = new QueryMinimalCriteriasModel
        {
            CriteriaFor = criteriaType,
        };
        
        var classCriteria = await GetMinimalCriterias(query);
        return classCriteria.Select(c => new CriteriaGradeModel
        {
            Id = c.Id,
            Name = c.Name,
            Weight = c.Weight,
            For = criteriaType
        }).ToList();
    }
}