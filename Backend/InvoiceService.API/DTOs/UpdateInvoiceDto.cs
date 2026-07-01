namespace InvoiceService.API.DTOs;

public class UpdateInvoiceDto
{
    public int InvoiceId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; }
}