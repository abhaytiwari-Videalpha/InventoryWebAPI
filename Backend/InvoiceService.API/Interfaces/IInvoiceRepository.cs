using InvoiceService.API.Models;
using InvoiceService.API.Helpers;

namespace InvoiceService.API.Interfaces;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetAllAsync();

    Task<Invoice?> GetByIdAsync(int id);

    Task<Invoice> CreateAsync(Invoice invoice);

    Task UpdateAsync(Invoice invoice);

    Task DeleteAsync(Invoice invoice);

    Task<(IEnumerable<Invoice> Invoices, int TotalRecords)>
        GetPagedInvoicesAsync(
            PaginationParameters parameters);
}