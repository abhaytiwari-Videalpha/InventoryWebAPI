namespace InvoiceItemService.API.Caching;

public interface ICacheService
{
    Task<T?> GetData<T>(string key);

    Task SetData<T>(
        string key,
        T value,
        TimeSpan? expiration = null);

    Task RemoveData(string key);
}