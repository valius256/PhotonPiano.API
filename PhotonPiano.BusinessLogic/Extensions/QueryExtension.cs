using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Extensions;

public static class QueryExtension
{
    public static PagedResult<T> ProcessData<T>(
        IEnumerable<T> data,
        int page,
        int pageSize,
        string sortColumn = null,
        bool orderByDesc = false,
        Func<T, bool>? filter = null)
    {
        // Apply filtering if a filter function is provided
        var filteredData = filter == null ? data.AsQueryable() : data.Where(filter).AsQueryable();

        // Apply sorting if a valid sort column is provided
        if (!string.IsNullOrEmpty(sortColumn))
        {
            var propertyInfo = typeof(T).GetProperty(sortColumn);
            if (propertyInfo != null)
                filteredData = orderByDesc
                    ? filteredData.OrderByDescending(x => propertyInfo.GetValue(x, null))
                    : filteredData.OrderBy(x => propertyInfo.GetValue(x, null));
            else
                throw new ArgumentException($"The property '{sortColumn}' does not exist on type '{typeof(T).Name}'");
        }

        // Paginate the data
        var totalItems = filteredData.Count();
        var paginatedItems = filteredData
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Return the paginated result
        return new PagedResult<T>
        {
            TotalCount = totalItems,
            Items = paginatedItems,
            Page = page,
            Limit = pageSize
        };
    }
}