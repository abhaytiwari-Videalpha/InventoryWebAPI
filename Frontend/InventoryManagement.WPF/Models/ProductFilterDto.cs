namespace InventoryManagement.WPF.Models;

public class ProductFilterDto
{
    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public bool LowStockOnly { get; set; }

    public string SortBy { get; set; } = "name";

    public string SortOrder { get; set; } = "asc";
}