using System.Linq.Expressions;
using Application.DTOs.Common;
using Application.Responses;
using Domain.Common;

namespace Application.Contracts.Persistence;

public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAsEnumerableAsync(Expression<Func<T, bool>> predicate = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
        List<Expression<Func<T, object>>> includes = null, 
        bool disableTracking = true);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

    Task<Paginated<T>> GetWherePaginated(PaginateQueryRequest<T> queryRequest);
    Task<Paginated<T>> GetPaginated(PaginateQueryRequestNew<T> queryRequest);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeString = null,
        bool disableTracking = true);
    
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true);

    Task<T> GetByIdAsync(Guid id);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<bool> AddMultipleAsync(IEnumerable<T> entities);

    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true);
    void Update(T entity);
    void DeleteAsync(T entity);
}