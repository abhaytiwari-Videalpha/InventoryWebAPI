using AutoMapper;
using InvoiceItemService.API.Caching;
using InvoiceItemService.API.DTOs;
using InvoiceItemService.API.Interfaces;
using InvoiceItemService.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using InvoiceItemService.API.Helpers;
using InvoiceItemService.API.Responses;

namespace InvoiceItemService.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class InvoiceItemsController : ControllerBase
{
    private readonly IInvoiceItemService _service;
    private readonly IProductApiService _productApiService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<InvoiceItemsController> _logger;

    public InvoiceItemsController(
        IInvoiceItemService service,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<InvoiceItemsController> logger,
        IProductApiService productApiService)
    {
        _service = service;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
        _productApiService = productApiService;
    }

    // GET: api/invoiceitems
    [HttpGet]
    [Authorize]
    public async Task<ActionResult>
        GetInvoiceItems(
            [FromQuery] PaginationParameters parameters)
    {
        var result =
            await _service.GetPagedInvoiceItems(parameters);

        var itemDtos =
            _mapper.Map<List<InvoiceItemDto>>(
                result.InvoiceItems);

        var response =
            new PagedResponse<List<InvoiceItemDto>>(
                true,
                "InvoiceItems fetched successfully",
                itemDtos,
                parameters.PageNumber,
                parameters.PageSize,
                result.TotalRecords
            );

        return Ok(response);
    }

    // GET: api/invoiceitems/1
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<InvoiceItemDto>>
        GetInvoiceItem(int id)
    {
        string cacheKey = $"invoiceitem_{id}";

        var cachedItem =
            await _cacheService.GetData<InvoiceItemDto>(cacheKey);

        if (cachedItem != null)
        {
            _logger.LogInformation(
                $"InvoiceItem {id} fetched from Redis Cache");

            return Ok(cachedItem);
        }

        _logger.LogInformation(
            $"InvoiceItem {id} fetched from Database");

        var item =
            await _service.GetInvoiceItemById(id);

        if (item == null)
            return NotFound();

        var itemDto =
            _mapper.Map<InvoiceItemDto>(item);

        await _cacheService.SetData(
            cacheKey,
            itemDto,
            TimeSpan.FromMinutes(5));

        return Ok(itemDto);
    }

    // POST: api/invoiceitems
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InvoiceItemDto>>
    CreateInvoiceItem(CreateInvoiceItemDto dto)
    {
        // Verify Product Exists
        var product =
            await _productApiService.GetProduct(dto.ProductId);

        if (product == null)
        {
            return BadRequest(new
            {
                Message =
                    $"Product with ID {dto.ProductId} not found."
            });
        }

        var item =
            _mapper.Map<InvoiceItem>(dto);

        // Auto-fill price from ProductService
        item.UnitPrice = product.Price;
        var created =
            await _service.CreateInvoiceItem(item);

        await _cacheService.RemoveData("invoiceitems_all");

        _logger.LogInformation(
            "InvoiceItems cache invalidated after Create");

        return CreatedAtAction(
            nameof(GetInvoiceItem),
            new { id = created.InvoiceItemId },
            _mapper.Map<InvoiceItemDto>(created)
        );
    }

    // PUT: api/invoiceitems/1
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        UpdateInvoiceItem(
            int id,
            UpdateInvoiceItemDto dto)
    {
        if (id != dto.InvoiceItemId)
            return BadRequest();

        var existing =
            await _service.GetInvoiceItemById(id);

        if (existing == null)
            return NotFound();

        var item =
            _mapper.Map<InvoiceItem>(dto);

        await _service.UpdateInvoiceItem(item);

        await _cacheService.RemoveData("invoiceitems_all");
        await _cacheService.RemoveData($"invoiceitem_{id}");

        _logger.LogInformation(
            $"Cache invalidated for InvoiceItem {id}");

        return Ok(new
        {
            Message = "Invoice Item updated successfully"
        });
    }

    // DELETE: api/invoiceitems/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult>
        DeleteInvoiceItem(int id)
    {
        var existing =
            await _service.GetInvoiceItemById(id);

        if (existing == null)
            return NotFound();

        await _service.DeleteInvoiceItem(id);

        await _cacheService.RemoveData("invoiceitems_all");
        await _cacheService.RemoveData($"invoiceitem_{id}");

        _logger.LogInformation(
            $"Cache invalidated after deleting InvoiceItem {id}");

        return Ok(new
        {
            Message = "Invoice Item deleted successfully"
        });
    }

    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("InvoiceItems Controller Working");
    }
}