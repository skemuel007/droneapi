namespace Application.DTOs.Common;

public class PaginatedQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? FilterColumn { get; set; }
    public string? FilterQuery { get; set; }
}