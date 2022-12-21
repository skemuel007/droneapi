using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Responses;

public class Paginated<T> : BaseCommandResponse
{
    private Paginated(
        List<T> data,
        int count,
        int pageIndex,
        int pageSize,
        string? sortColumn,
        string? sortOrder,
        string? filterColumn = null,
        string? filterQuery = null)
    {
        Data = data;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        SortColumn = sortColumn;
        SortOrder = sortOrder;
        FilterColumn = filterColumn;
        FilterQuery = filterQuery;
    }

    public static async Task<Paginated<T>> ToPaginatedList(
        List<T> data,
        int count,
        int pageIndex,
        int pageSize,
        string? sortColumn,
        string? sortOrder,
        string? filterColumn = null,
        string? filterQuery = null)
    {
        return new Paginated<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
    }
    /*public static async Task<Paginated<T>> ToPagedList(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null)
    {

        if (!string.IsNullOrEmpty(filterColumn) &&
            !string.IsNullOrEmpty(filterQuery) &&
            IsValidProperty(filterColumn))
        {
            source = source.Where(string.Format("{0}.StartsWith(@0)", filterColumn, filterQuery));
        }
        var count = await source.CountAsync();

        if (!string.IsNullOrEmpty(sortColumn) &&
            IsValidProperty(sortColumn))
        {
            sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC"
                ? "ASC"
                : "DESC";
            source = source.OrderBy(string.Format("{0}", sortColumn, sortOrder)); 

        }
        
        source = source.Skip((pageIndex - 1) * pageSize)                                         
            .Take(pageSize);                                                                
                                                                                               
        var data = await source.ToListAsync();                                             
                                                                                               
        return new Paginated<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);  
    }

    public static bool IsValidProperty(string propertyName,
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
    }*/

    public List<T> Data { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public bool HasPreviousPage
    {
        get
        {
            return (PageIndex > 1);
        }
    }

    public bool HasNextPage
    {
        get
        {
            return (PageIndex < TotalPages);
        }
    }

    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? FilterColumn { get; set; }
    public string? FilterQuery { get; set; }
}

