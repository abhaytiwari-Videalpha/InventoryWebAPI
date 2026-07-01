namespace InvoiceItemService.API.DTOs;

public class ProductResponseDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}