using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
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

    public async Task<PagedResult<CriteriaModel>> GetPagedCriteria(QueryCriteriaModel query)
    {
        // var cacheCriteria =
        //     await _serviceFactory.RedisCacheService.GetAsync<PagedResult<CriteriaDetailModel>>("criteria");
        // if (cacheCriteria is not null) return cacheCriteria;

        var (page, pageSize, sortColumn, orderByDesc, keyword)
            = query;
        var likeKeyword = query.GetLikeKeyword();
        var result = await _unitOfWork.CriteriaRepository.GetPaginatedWithProjectionAsync<CriteriaModel>(page,
            pageSize,
            sortColumn, orderByDesc,
            expressions:
            [
                c => string.IsNullOrEmpty(keyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(c.Name), likeKeyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(c.Description ?? string.Empty), likeKeyword)
            ]);
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

    public async Task<CriteriaModel> CreateCriteria(CreateCriteriaModel createCriteriaModel, string userFirebaseId)
    {
        if (createCriteriaModel.Weight <= 0 || createCriteriaModel.Weight > 100)
        {
            throw new BadRequestException("Criteria weight must be from 1 - 100");
        }

        var criteria = createCriteriaModel.Adapt<Criteria>();
        criteria.CreatedById = userFirebaseId;
        //Shift criteria
        var otherCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == criteria.For);
        var deltaWeight = 100 - createCriteriaModel.Weight;

        foreach (var c in otherCriteria)
        {
            c.Weight = c.Weight * deltaWeight / 100;
        }

        var created = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var createdCriteria = await _unitOfWork.CriteriaRepository.AddAsync(criteria);
            await _unitOfWork.CriteriaRepository.UpdateRangeAsync(otherCriteria);
            await _unitOfWork.SaveChangesAsync();
            return createdCriteria;
        });
        
        return created.Adapt<CriteriaModel>();
    }

    public async Task UpdateCriteria(BulkUpdateCriteriaModel bulkUpdateCriteriaModel, string userFirebaseId)
    {
        var criteriaList = new List<Criteria>();
        foreach (var updateModel in bulkUpdateCriteriaModel.UpdateCriteria)
        {
            var criteria = await _unitOfWork.CriteriaRepository.GetByIdAsync(updateModel.Id);
            if (criteria == null)
            {
                throw new NotFoundException($"Criteria {updateModel.Id} not found");
            }
            updateModel.Adapt(criteria);
            criteria.UpdateById = userFirebaseId;
            criteria.UpdatedAt = DateTime.UtcNow.AddHours(7);
            criteriaList.Add(criteria);
        }

        var allCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == bulkUpdateCriteriaModel.For);
        if (allCriteria.Sum(c => c.Weight) != 100)
        {
            throw new BadRequestException("Weight of all criteria is not equal 100");
        }

        await _unitOfWork.CriteriaRepository.UpdateRangeAsync(criteriaList);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCriteria(Guid id, string userFirebaseId)
    {
        var criteria = await _unitOfWork.CriteriaRepository.GetByIdAsync(id);
        if (criteria == null)
        {
            throw new NotFoundException($"Criteria not found");
        }
        criteria.DeletedById = userFirebaseId;
        criteria.DeletedAt = DateTime.UtcNow.AddHours(7);
        criteria.RecordStatus = DataAccess.Models.Enum.RecordStatus.IsDeleted;

        //Shift criteria
        var otherCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == criteria.For);
        var deltaWeight = 100 - criteria.Weight;

        foreach (var c in otherCriteria)
        {
            c.Weight = c.Weight * 100 / deltaWeight;
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.CriteriaRepository.UpdateAsync(criteria);
            await _unitOfWork.CriteriaRepository.UpdateRangeAsync(otherCriteria);
            await _unitOfWork.SaveChangesAsync();
        });

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