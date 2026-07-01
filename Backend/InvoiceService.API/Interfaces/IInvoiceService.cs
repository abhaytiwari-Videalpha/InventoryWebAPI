using InvoiceService.API.Helpers;
using InvoiceService.API.Models;

namespace InvoiceService.API.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<Invoice>> GetAllInvoices();

    Task<Invoice?> GetInvoiceById(int id);

    Task<Invoice> CreateInvoice(Invoice invoice);

    Task UpdateInvoice(Invoice invoice);

    Task DeleteInvoice(int id);

    Task<(IEnumerable<Invoice> Invoices, int TotalRecords)>
        GetPagedInvoices(PaginationParameters parameters);
}