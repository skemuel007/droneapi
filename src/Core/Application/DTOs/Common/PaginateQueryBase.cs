using System.Linq.Expressions;

namespace Application.DTOs.Common;

public class PaginateQueryBase<T> : PaginatedQueryParams where T : class
{
    public List<Expression<Func<T, object>>> Includes { get; set; } = null;
    public Expression<Func<T, bool>> Predicate { get; set; } = null;
    public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; } = null;
    public string IncludeString { get; set; } = null;
    public bool DisableTracking { get; set; } = true;

}