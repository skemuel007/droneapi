using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Application.Contracts.Persistence;
using Application.DTOs.Common;
using Application.Responses;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DronesAppContext _dbContext;
    internal DbSet<T> _dbSet;

    public Repository(DronesAppContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = dbContext.Set<T>();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAsEnumerableAsync(Expression<Func<T, bool>> predicate = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
        List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
    {
        var query = await GetQueryable(predicate: predicate, orderBy: orderBy, includes: includes,
            disableTracking: disableTracking);
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null,
        bool disableTracking = true)
    {
        var query = await GetQueryable(predicate: predicate, orderBy: orderBy, includeString: includeString,
            disableTracking: disableTracking);
        return await query.ToListAsync();
        
    }

    public async Task<Paginated<T>> GetWherePaginated(PaginateQueryRequest<T> queryRequest)
    {
        IQueryable<T> query = _dbSet;
        query = query.AsNoTracking();

        if (queryRequest.Includes != null)
            query = queryRequest.Includes.Aggregate(query, (current, include) => current.Include(include));

        /*if (queryRequest.Filter != null)
            query = query.Where(queryRequest.Filter);

        if (queryRequest.OrderBy != null)
            query = queryRequest.OrderBy(query);*/

        if (queryRequest.Predicate != null)
            query = query.Where(queryRequest.Predicate);

        if (queryRequest.Includes != null)
            query = queryRequest.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (!string.IsNullOrEmpty(queryRequest.IncludeString))
            query = query.Include(queryRequest.IncludeString);


        if (queryRequest.OrderBy != null)
            query =  queryRequest.OrderBy(query);

        if (!string.IsNullOrEmpty(queryRequest.FilterColumn) &&
            !string.IsNullOrEmpty(queryRequest.FilterQuery) &&
            IsValidProperty(queryRequest.FilterColumn))
        {
            query = query.Where(string.Format("{0}.StartsWith(@0)", queryRequest.FilterColumn, queryRequest.FilterQuery));
        }
        var count = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryRequest.SortColumn) &&
            IsValidProperty(queryRequest.SortColumn))
        {
            queryRequest.SortOrder = !string.IsNullOrEmpty(queryRequest.SortOrder) && queryRequest.SortOrder.ToUpper() == "ASC"
                ? "ASC"
                : "DESC";
            query = query.OrderBy(string.Format("{0} {1}", queryRequest.SortColumn, queryRequest.SortOrder)); 

        } else
        {
            query = query.OrderBy(x => x.Id);
        }
        
        query = query.Skip((queryRequest.Page - 1) * queryRequest.PageSize)                                         
            .Take(queryRequest.PageSize);                                                                
                                                                                               
        var data = await query.ToListAsync();
        
        // do pagination
        /*return await Paginated<T>.ToPagedList(
            query,
            pageIndex: queryRequest.Page,
            pageSize: queryRequest.PageSize,
            sortOrder: queryRequest.SortOrder,
            sortColumn: queryRequest.SortColumn,
            filterColumn: queryRequest.FilterColumn,
            filterQuery: queryRequest.FilterQuery);*/
        
        return Paginated<T>.ToPaginatedList(
            data: data,
            count: count,
            pageIndex: queryRequest.Page,
            pageSize: queryRequest.PageSize,
            sortOrder: queryRequest.SortOrder,
            sortColumn: queryRequest.SortColumn,
            filterColumn: queryRequest.FilterColumn,
            filterQuery: queryRequest.FilterQuery);

    }
    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
    {
        var query = await GetQueryable(predicate: predicate, orderBy: orderBy, includes: includes,
            disableTracking: disableTracking);
        return await query.ToListAsync();
    }

    public async Task<Paginated<T>> GetPaginated(PaginateQueryRequestNew<T> queryRequest)
    {
        var query = await GetQueryable(predicate: queryRequest.Predicate, orderBy: queryRequest.OrderBy, includes: queryRequest.Includes,
            disableTracking: queryRequest.DisableTracking);


        query = query.Skip((queryRequest.Page - 1) * queryRequest.PageSize)
            .Take(queryRequest.PageSize);

        var count = await query.CountAsync();

        var data = await query.ToListAsync();

        return Paginated<T>.ToPaginatedList(
            data: data,
            count: count,
            pageIndex: queryRequest.Page,
            pageSize: queryRequest.PageSize,
            sortOrder: null,
            sortColumn: null,
            filterColumn: null,
            filterQuery: null);

    }

    private async Task<IQueryable<T>> GetQueryable(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, string includeString = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = _dbSet;
        if (disableTracking)
            query = query.AsNoTracking();

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        
        if (!string.IsNullOrEmpty(includeString))
            query = query.Include(includeString);

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            return orderBy(query);

        return query;
    }

    public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true)
    {
        var query = await GetQueryable(predicate: predicate, orderBy: orderBy, includes: includes,
            disableTracking: disableTracking);
        return await query.SingleOrDefaultAsync();
    }
    
    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true)
    {
        var query = await GetQueryable(predicate: predicate, orderBy: orderBy, includes: includes,
            disableTracking: disableTracking);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        var entityEntry = await _dbSet.AddAsync(entity);
        return entityEntry.Entity;
    }

    public async Task<bool> AddMultipleAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return true;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AnyAsync(predicate);

    public virtual void Update(T entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public virtual void DeleteAsync(T entity)
    {
        if (_dbContext.Entry(entity: entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity: entity);
        }
        _dbSet.Remove(entity: entity);
    }
    
    
    private static bool IsValidProperty(string propertyName,
        bool throwExceptionIfNotFound = true)
    {
        var prop = typeof(T).GetProperty(propertyName,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);

        if (prop == null && throwExceptionIfNotFound)
            throw new NotSupportedException(
                string.Format($"Error: Property '{propertyName}' does not exists."));
        return prop != null;
    }
}