namespace IdentityService.API.Responses;

public class PagedResponse<T> : ApiResponse<T>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public PagedResponse(
        bool success,
        string message,
        T data,
        int pageNumber,
        int pageSize,
        int totalRecords)
        : base(success, message, data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;

        TotalPages =
            (int)Math.Ceiling(
                totalRecords /
                (double)pageSize);
    }
}