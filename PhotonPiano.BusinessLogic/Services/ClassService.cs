

using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Paging;
using Microsoft.EntityFrameworkCore;

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
            //foreach (var item in result.Items)
            //{
            //    item.Capacity = int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");
            //}
            return result;
        }
    }
}
