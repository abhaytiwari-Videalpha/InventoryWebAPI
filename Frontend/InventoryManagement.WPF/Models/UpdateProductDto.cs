namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.DTOs.UpdateProductDto.
    /// Used for PUT /api/v1/Products/{id}.
    /// </summary>
    public class UpdateProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}