using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class EntranceTestStudentService : IEntranceTestStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceFactory _serviceFactory;

    public EntranceTestStudentService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }


    public async Task<PagedResult<EntranceTestStudentWithEntranceTestAndStudentAccountModel>> GetPagedEntranceTest(QueryEntranceTestStudentModel query)
    {
        var (page, pageSize, sortColumn, orderByDesc, 
            userFirebaseIds, bandScores , entranceTestIds)
            = query;
        
        var result = await _unitOfWork.EntranceTestStudentRepository
            .GetPaginatedWithProjectionAsync<EntranceTestStudentWithEntranceTestAndStudentAccountModel>(
                page, pageSize, sortColumn, orderByDesc,
                expressions:
                [
                    e => userFirebaseIds != null && (userFirebaseIds.Count == 0 || userFirebaseIds.Contains(e.StudentFirebaseId)),
                    e => entranceTestIds != null && (entranceTestIds.Count == 0 || entranceTestIds.Contains(e.EntranceTestId)),
                    e => bandScores != null && (bandScores.Count == 0 || bandScores.Contains(e.BandScore!.Value))

                ]);
        
        return result;
    }

    public async Task<EntranceTestStudentWithEntranceTestAndStudentAccountModel> GetEntranceTestStudentDetailById(Guid id)
    {
        var entranceTestStudent = await _unitOfWork.EntranceTestStudentRepository.FindSingleProjectedAsync<EntranceTestStudentWithEntranceTestAndStudentAccountModel>(
            q => q.Id == id,
            hasTrackings: false);
        if (entranceTestStudent is null)
        {
            throw new NotFoundException("entranceTestStudent not found.");
        }
        
        return entranceTestStudent;
    }

    public async Task<EntranceTestStudentWithEntranceTestAndStudentAccountModel> CreateEntranceTestStudent(EntranceTestStudentModel entranceTestStudent, string currentUserFirebaseId)
    {
        var etsModel = entranceTestStudent.Adapt<EntranceTestStudent>();
        etsModel.CreatedById = currentUserFirebaseId;
       
        // waiting for implementt EntranceTestService to check is model Exist
        // var currentEntranceTestModel = _serviceFactory.EntranceTestService.GetEntranceTestById(etsModel.EntranceTestId);
        // if (etsModel.EntranceTestId is null)
        // {
        //     throw new BadRequestException("EntranceTest is not exist please try again");
        // }
        
        var createdEntranceTestStudentId = Guid.Empty;
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var createdModel =  await _unitOfWork.EntranceTestStudentRepository.AddAsync(etsModel);
            await _unitOfWork.SaveChangesAsync();

            createdEntranceTestStudentId = createdModel.Id;
        });


        return await GetEntranceTestStudentDetailById(createdEntranceTestStudentId);
    }

    public async Task DeleteEntranceTestStudent(Guid id, string? currentUserFirebaseId = default)
    {
        var entranceTestStudentEntity = await _unitOfWork.EntranceTestStudentRepository.FindSingleAsync(q => q.Id == id);
        if (entranceTestStudentEntity is null)
        {
            throw new NotFoundException("This entranceTestStudent not found.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestStudentRepository.DeleteAsync(entranceTestStudentEntity);
        });
    }

    public async Task UpdateEntranceTestStudent(Guid id, UpdateEntranceTestStudentModel entranceTestStudent,
        string? currentUserFirebaseId = default)
    {
        var updatedEntranceTestStudentEntity = await _unitOfWork.EntranceTestStudentRepository.FindSingleAsync(q => q.Id == id);

        if (updatedEntranceTestStudentEntity is null)
        {
            throw new NotFoundException($"EntranceTestStudentEntity with id: {id} not found.");
        }
        
        entranceTestStudent.Adapt(updatedEntranceTestStudentEntity);
        updatedEntranceTestStudentEntity.UpdateById = currentUserFirebaseId;
        updatedEntranceTestStudentEntity.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
    }
}