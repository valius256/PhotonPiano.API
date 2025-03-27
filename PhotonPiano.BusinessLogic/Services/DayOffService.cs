
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.DayOff;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Services
{
    public class DayOffService : IDayOffService
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IUnitOfWork _unitOfWork;

        public DayOffService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
        }
        public async Task<PagedResult<DayOffModel>> GetPagedDayOffs(QueryDayOffModel queryDayOff)
        {
            var likeKeyword = queryDayOff.GetLikeKeyword();

            var startTime = queryDayOff.StartTime != null ? DateTime.SpecifyKind(queryDayOff.StartTime.Value, DateTimeKind.Utc) : (DateTime?)null;
            var endTime = queryDayOff.EndTime != null ? DateTime.SpecifyKind(queryDayOff.EndTime.Value, DateTimeKind.Utc) : (DateTime?)null;

            var result = await _unitOfWork.DayOffRepository
                .GetPaginatedWithProjectionAsync<DayOffModel>(
                    queryDayOff.Page, queryDayOff.PageSize, queryDayOff.SortColumn, queryDayOff.OrderByDesc,
                    expressions:
                    [
                        d => !startTime.HasValue || d.StartTime >= startTime,
                        d => !endTime.HasValue || d.StartTime >= endTime,
                        // search 
                        d => string.IsNullOrEmpty(queryDayOff.Name) ||
                             EF.Functions.ILike(EF.Functions.Unaccent(d.Name ?? ""), likeKeyword)
                        ]);

            // await _serviceFactory.RedisCacheService
            //     .SaveAsync($"entranceTests_page{query.Page}_pageSize{query.PageSize}_keyword{query.Keyword}",
            //         result, TimeSpan.FromHours(3));
            return result;
        }
    }
}
