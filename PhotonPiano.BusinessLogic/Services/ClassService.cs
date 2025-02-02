

using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Paging;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFactory _serviceFactory;

        public ClassService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
        }

        public async Task<ClassDetailModel> GetClassDetailById(Guid id)
        {
            var classDetail = await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassDetailModel>(c => c.Id == id);
            if ( classDetail is null)
            {
                throw new NotFoundException("Class not found");
            }
            return classDetail;
        }

        public async Task<PagedResult<ClassModel>> GetPagedClasses(QueryClassModel queryClass)
        {
            var (page, pageSize, sortColumn, orderByDesc,
                classStatus, level, keyword, isScorePublished,teacherId,studentId) = queryClass;

            string likeKeyword = queryClass.GetLikeKeyword();

            var result = await _unitOfWork.ClassRepository.GetPaginatedWithProjectionAsync<ClassModel>(
                page, pageSize, sortColumn, orderByDesc,
                expressions: [
                    q => classStatus.Count == 0 || classStatus.Contains(q.Status),
                    q => level.Count == 0 || level.Contains(q.Level ?? DataAccess.Models.Enum.Level.Beginner),
                    q => !isScorePublished.HasValue || q.IsScorePublished == isScorePublished,
                    q => teacherId == null || q.InstructorId == teacherId,
                    q => studentId == null || q.StudentClasses.Any(sc => sc.StudentFirebaseId == studentId),
                    q =>
                        string.IsNullOrEmpty(keyword) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(q.Name), likeKeyword)
                ]
            );
            // Fetch the class capacity
            int capacity = int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

            // Fetch students of the classes in one query
            var classIds = result.Items.Select(c => c.Id).ToList();
            var studentClasses = await _unitOfWork.StudentClassRepository
                .FindAsync(sc => classIds.Contains(sc.ClassId));
                
            var countDictionary = studentClasses
                .GroupBy(c => c.ClassId)
                .Select(g => new {ClassId = g.Key, Count = g.Count() })
                .ToDictionary(c => c.ClassId, c => c.Count);


            // Update items efficiently
            result.Items = result.Items.Select(item => item with
            {
                Capacity = capacity,
                StudentNumber = countDictionary.TryGetValue(item.Id, out var count) ? count : 0
            }).ToList();

            
            return result;
        }
    }
}
