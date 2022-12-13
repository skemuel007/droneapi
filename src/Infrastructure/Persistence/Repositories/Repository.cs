using System.Linq.Expressions;
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

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = _dbSet;
        if (disableTracking)
            query = query.AsNoTracking();

        if (!string.IsNullOrEmpty(includeString))
            query = query.Include(includeString);

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        
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
        
        // do pagination
        return await Paginated<T>.ToPagedList(
            query,
            pageIndex: queryRequest.Page,
            pageSize: queryRequest.PageSize,
            sortOrder: queryRequest.SortOrder,
            sortColumn: queryRequest.SortColumn,
            filterColumn: queryRequest.FilterColumn,
            filterQuery: queryRequest.FilterQuery);


    }
    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
    {
        IQueryable<T> query = _dbSet;
        if (disableTracking)
            query = query.AsNoTracking();

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        
        return await query.ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

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
}