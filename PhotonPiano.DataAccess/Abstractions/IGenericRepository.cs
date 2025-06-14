﻿using Microsoft.EntityFrameworkCore.Query;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using System.Linq.Expressions;

namespace PhotonPiano.DataAccess.Abstractions;

public interface IGenericRepository<T> where T : BaseEntity
{
    IQueryable<T> Entities { get; }

    Task<List<T>> FindAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true,
        bool ignoreQueryFilters = false);

    IQueryable<T> FindAsQueryable(Expression<Func<T, bool>> predicate, bool hasTrackings = true);

    Task<T?> FindFirstAsync(Expression<Func<T, bool>> expression,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        Expression<Func<T, object>>? orderByExpression = default,
        bool orderByDescending = true);

    Task<T?> FindSingleAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true,
        bool ignoreQueryFilters = false);

    Task<List<TProjectTo>> FindProjectedAsync<TProjectTo>(Expression<Func<T, bool>>? expression = default,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default);

    Task<TProjectTo?> FindFirstProjectedAsync<TProjectTo>(Expression<Func<T, bool>> expression,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default);

    Task<TProjectTo?> FindSingleProjectedAsync<TProjectTo>(Expression<Func<T, bool>> expression,
        bool hasTrackings = true,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default,
        bool hasSplitQuery = false);

    Task<T?> GetByIdAsync(Guid id);

    Task<List<T>> GetAllAsync(bool hasTrackings = true, bool ignoreQueryFilters = false);

    Task<T> AddAsync(T TEntity);

    Task UpdateAsync(T TEntity);

    Task DeleteAsync(T TEntity);

    Task DeleteRangeAsync(IEnumerable<T> entities);

    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

    Task AddRangeAsync(IEnumerable<T> entities);

    Task UpdateRangeAsync(IEnumerable<T> entities);
    Task ExecuteDeleteAsync(Expression<Func<T, bool>> expression);

    Task<int> ExecuteUpdateAsync(Expression<Func<T, bool>> expression,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateExpression,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<T, bool>>? expression, bool hasTrackings = true,
        bool ignoreQueryFilters = false);

    Task<PagedResult<T>> GetPaginatedAsync(int pageNumber, int pageSize, string sortColumn, bool desc,
        bool hasTrackings = false, bool ignoreQueryFilters = false,
        params List<Expression<Func<T, bool>>?> expressions);

    Task<PagedResult<TProjectTo>> GetPaginatedWithProjectionAsync<TProjectTo>
    (int pageNumber, int pageSize, string sortColumn, bool desc, bool hasTrackings = false,
        bool ignoreQueryFilters = false,
        TrackingOption option = TrackingOption.Default,
        bool hasSplitQuery = false,
        params List<Expression<Func<T, bool>>?> expressions);

    IQueryable<T> GetPaginatedWithProjectionAsQueryable<TProjectTo>(int pageNumber,
        int pageSize, string sortColumn, bool desc,
        bool hasTrackings = false,
        bool ignoreQueryFilters = false, TrackingOption option = TrackingOption.Default,
        bool hasSplitQuery = false,
        params List<Expression<Func<T, bool>>?> expressions);

    void Detach(T entity);

    void ClearChangeTracker();

    void Attach(T entity);

    void MarkAsModified(T entity);
}