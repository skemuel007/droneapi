using System.Linq.Expressions;

namespace Application.DTOs.Common;

public class PaginateQueryBase<T> : PaginatedQueryParams where T : class
{
    public List<Expression<Func<T, object>>> Includes { get; set; } = null;
}