using System.Collections;
using System.Linq.Expressions;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Extensions;

public static class QueryExtension
{
    public static PagedResult<T> ProcessData<T>(
        IEnumerable<T> data,
        int page,
        int pageSize,
        string? sortColumn = null,
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

    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var combined = new ReplaceParameterVisitor
        {
            { expr1.Parameters[0], parameter },
            { expr2.Parameters[0], parameter }
        }.Visit(Expression.AndAlso(expr1.Body, expr2.Body));

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private class ReplaceParameterVisitor : ExpressionVisitor, IEnumerable
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map = new();

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Add(ParameterExpression from, ParameterExpression to)
        {
            _map[from] = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_map.TryGetValue(node, out var replacement)) node = replacement;
            return base.VisitParameter(node);
        }
    }
}