namespace InventoryManagement.WPF.Models;

public class ProductResponse
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<Product> Data { get; set; } = new();
}