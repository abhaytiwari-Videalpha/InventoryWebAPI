namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.DTOs.ProductDto.
    /// DO NOT MODIFY without backend changes.
    /// </summary>
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}