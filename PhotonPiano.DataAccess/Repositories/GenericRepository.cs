using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.DataAccess.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Entities => _context.Set<T>();

    public Task<T> AddAsync(T TEntity)
    {
        _context.Add(TEntity);
        return Task.FromResult(TEntity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().AnyAsync(expression);
    }

    public async Task DeleteAsync(T TEntity)
    {
        TEntity.DeletedAt = DateTime.UtcNow.AddHours(7);
        TEntity.RecordStatus = RecordStatus.IsDeleted;
        _context.Update(TEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.DeletedAt = DateTime.UtcNow.AddHours(7);
            entity.RecordStatus = RecordStatus.IsDeleted;
        }

        _context.UpdateRange(entities);
        await _context.SaveChangesAsync();
    }


    public async Task ExecuteDeleteAsync(Expression<Func<T, bool>> expression)
    {
        await _context.Set<T>().Where(expression)
            .ExecuteUpdateAsync(set => set
                .SetProperty(e => e.DeletedAt, e => DateTime.UtcNow.AddHours(7))
                .SetProperty(e => e.RecordStatus, e => RecordStatus.IsDeleted));
    }

    public async Task<int> ExecuteUpdateAsync(Expression<Func<T, bool>> expression, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>()
            .Where(expression)
            .ExecuteUpdateAsync(updateExpression, cancellationToken);
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true,
        bool ignoreQueryFilters = false)
    {
        var query = hasTrackings
            ? _context.Set<T>().Where(expression)
            : _context.Set<T>().AsNoTracking().Where(expression);

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return await query.ToListAsync();
    }

    public IQueryable<T> FindAsQueryable(Expression<Func<T, bool>> predicate, bool hasTrackings = true)
    {
        return hasTrackings
            ? _context.Set<T>().Where(predicate)
            : _context.Set<T>().AsNoTracking().Where(predicate);
    }

    public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> expression,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        Expression<Func<T, object>>? orderByExpression = default, 
        bool orderByDescending = true)
    {
        var query = hasTrackings
            ? _context.Set<T>().Where(expression)
            : _context.Set<T>().AsNoTracking().Where(expression);

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        if (orderByExpression is not null) 
        {
            query = orderByDescending ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> FindSingleAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true,
        bool ignoreQueryFilters = false)
    {
        var query = hasTrackings
            ? _context.Set<T>().Where(expression)
            : _context.Set<T>().AsNoTracking().Where(expression);

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return await query.SingleOrDefaultAsync();
    }

    public async Task<List<TProjectTo>> FindProjectedAsync<TProjectTo>(
        Expression<Func<T, bool>>? expression = default,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (!hasTrackings)
            query = option switch
            {
                TrackingOption.IdentityResolution => query.AsNoTrackingWithIdentityResolution(),
                _ => query.AsNoTracking()
            };

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return expression is not null
            ? await query.Where(expression).ProjectToType<TProjectTo>().ToListAsync()
            : await query.ProjectToType<TProjectTo>().ToListAsync();
    }

    public async Task<TProjectTo?> FindFirstProjectedAsync<TProjectTo>(Expression<Func<T, bool>> expression,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (!hasTrackings)
            query = option switch
            {
                TrackingOption.IdentityResolution => query.AsNoTrackingWithIdentityResolution(),
                _ => query.AsNoTracking()
            };

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return await query.Where(expression).ProjectToType<TProjectTo>().FirstOrDefaultAsync();
    }

    public async Task<TProjectTo?> FindSingleProjectedAsync<TProjectTo>(Expression<Func<T, bool>> expression,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (!hasTrackings)
            query = option switch
            {
                TrackingOption.IdentityResolution => query.AsNoTrackingWithIdentityResolution(),
                _ => query.AsNoTracking()
            };

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return await query.Where(expression).ProjectToType<TProjectTo>().SingleOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync(bool hasTrackings = true, bool ignoreQueryFilters = false)
    {
        var query = hasTrackings
            ? _context.Set<T>()
            : _context.Set<T>().AsNoTracking();

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public Task UpdateAsync(T TEntity)
    {
        _context.Set<T>().Update(TEntity);
        return Task.CompletedTask;
    }
    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        var enumerable = entities as T[] ?? entities.ToArray();
        if (enumerable.Any())
        {
            _context.Set<T>().UpdateRange(enumerable);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? expression, bool hasTrackings = true,
        bool ignoreQueryFilters = false)
    {
        var query = hasTrackings
            ? _context.Set<T>()
            : _context.Set<T>().AsNoTracking();

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        return await query.CountAsync(expression ?? (o => true));
    }

    public async Task<PagedResult<T>> GetPaginatedAsync(int pageNumber, int pageSize, string sortColumn, bool desc,
        bool hasTrackings = false,
        bool ignoreQueryFilters = false,
        params List<Expression<Func<T, bool>>?> expressions)
    {
        // Validate the sortColumn
        var property = typeof(T).GetProperty(sortColumn);
        if (property is null)
            throw new BadRequestException($"Property '{sortColumn}' does not exist on type '{typeof(T).Name}'.");

        // Base query with optional tracking and filtering
        var query = hasTrackings
            ? _context.Set<T>()
            : _context.Set<T>().AsNoTracking();

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        //Filter
        if (expressions.Count != 0)
            query = expressions.Aggregate(query,
                (current, expression) => expression is not null ? current.Where(expression) : current);

        // Build the sorting expression dynamically
        var parameter = Expression.Parameter(typeof(T), "t");
        var propertyAccess = Expression.Property(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        // Apply sorting
        query = desc
            ? Queryable.OrderByDescending(query, (dynamic)orderByExpression)
            : Queryable.OrderBy(query, (dynamic)orderByExpression);

        // Paginate and return results
        return new PagedResult<T>
        {
            TotalCount = await query.CountAsync(),
            Page = pageNumber,
            Limit = pageSize,
            Items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
        };
    }

    public async Task<PagedResult<TProjectTo>> GetPaginatedWithProjectionAsync<TProjectTo>(int pageNumber,
        int pageSize, string sortColumn, bool desc,
        bool hasTrackings = false,
        bool ignoreQueryFilters = false,
        params List<Expression<Func<T, bool>>?> expressions)
    {
        var query = GetPaginatedWithProjectionAsQueryable<TProjectTo>(pageNumber, pageSize, sortColumn, desc, hasTrackings, ignoreQueryFilters, expressions);

        // Paginate and return results
        return new PagedResult<TProjectTo>
        {
            TotalCount = await query.CountAsync(),
            Page = pageNumber,
            Limit = pageSize,
            Items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ProjectToType<TProjectTo>()
                .ToListAsync()
        };
    }

    public IQueryable<T> GetPaginatedWithProjectionAsQueryable<TProjectTo>(int pageNumber,
        int pageSize, string sortColumn, bool desc,
        bool hasTrackings = false,
        bool ignoreQueryFilters = false,
        params List<Expression<Func<T, bool>>?> expressions)
    {
        // Validate the sortColumn
        var property = typeof(T).GetProperty(sortColumn);
        if (property is null)
            throw new BadRequestException($"Property '{sortColumn}' does not exist on type '{typeof(T).Name}'.");

        // Base query with optional tracking and filtering
        var query = hasTrackings
            ? _context.Set<T>()
            : _context.Set<T>().AsNoTracking();

        query = ignoreQueryFilters ? query.IgnoreQueryFilters() : query;

        //Filter
        if (expressions.Count != 0)
            query = expressions.Aggregate(query,
                (current, expression) => expression is not null ? current.Where(expression) : current);

        // Build the sorting expression dynamically
        var parameter = Expression.Parameter(typeof(T), "t");
        var propertyAccess = Expression.Property(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        // Apply sorting
        query = desc
            ? Queryable.OrderByDescending(query, (dynamic)orderByExpression)
            : Queryable.OrderBy(query, (dynamic)orderByExpression);

        // Paginate and return results
        return query;
    }
}