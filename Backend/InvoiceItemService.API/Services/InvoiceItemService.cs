using InvoiceItemService.API.Interfaces;
using InvoiceItemService.API.Models;
using InvoiceItemService.API.Helpers;

namespace InvoiceItemService.API.Services;

public class InvoiceItemService : IInvoiceItemService
{
    private readonly IInvoiceItemRepository _repository;

    public InvoiceItemService(
        IInvoiceItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<InvoiceItem>>
        GetAllInvoiceItems()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<InvoiceItem?>
        GetInvoiceItemById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<InvoiceItem>
        CreateInvoiceItem(InvoiceItem invoiceItem)
    {
        return await _repository.CreateAsync(invoiceItem);
    }

    public async Task UpdateInvoiceItem(
        InvoiceItem invoiceItem)
    {
        await _repository.UpdateAsync(invoiceItem);
    }

    public async Task DeleteInvoiceItem(int id)
    {
        var item =
            await _repository.GetByIdAsync(id);

        if (item != null)
        {
            await _repository.DeleteAsync(item);
        }
    }

    public async Task<(IEnumerable<InvoiceItem> InvoiceItems, int TotalRecords)>
    GetPagedInvoiceItems(
        PaginationParameters parameters)
    {
        return await _repository
            .GetPagedInvoiceItemsAsync(parameters);
    }
}