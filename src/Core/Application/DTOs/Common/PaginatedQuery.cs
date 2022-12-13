using System.Linq.Expressions;
using Domain.Enums;

namespace Application.DTOs.Common;

public class PaginatedQuery<T> : PaginateQueryBase<T> where T : class
{
    public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; } = null;
    public Expression<Func<T, bool>> Filter { set; get; } = null;

}