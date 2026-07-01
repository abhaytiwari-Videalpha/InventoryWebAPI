namespace InvoiceService.API.DTOs;

public class CreateInvoiceDto
{
    public DateTime InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; }
}