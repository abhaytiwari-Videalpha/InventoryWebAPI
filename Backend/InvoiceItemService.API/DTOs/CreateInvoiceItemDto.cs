namespace InvoiceItemService.API.DTOs;

public class CreateInvoiceItemDto
{
    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}