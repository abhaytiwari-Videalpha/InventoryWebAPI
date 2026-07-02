namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.DTOs.CreateProductDto.
    /// Used for POST /api/v1/Products.
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}