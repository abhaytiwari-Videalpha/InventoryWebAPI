namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.Responses.PagedResponse<T>.
    /// </summary>
    /// <typeparam name="T">Response payload type.</typeparam>
    public class PagedResponse<T> : ApiResponse<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
    }
}