namespace InventoryManagement.WPF.Models;

public class CreateProductRequest
{
    public string name {get; set; } = string.Empty;

    public decimal Price {get; set; }

    public int Quantity {get; set; }
}