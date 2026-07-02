namespace ProductService.API.Helpers;

public class PaginationParameters
{
    private const int MaxPageSize = 100;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    // Search
    public string? Search { get; set; }

    // Sorting
    public string? SortBy { get; set; }

    public string SortOrder { get; set; } = "asc";

    // Filtering
    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public bool? LowStockOnly { get; set; }
}