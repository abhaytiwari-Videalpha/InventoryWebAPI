using AutoMapper;
using InvoiceService.API.DTOs;
using InvoiceService.API.Interfaces;
using InvoiceService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoiceService.API.Caching;
using Asp.Versioning;
using InvoiceService.API.Helpers;
using InvoiceService.API.Responses;

namespace InvoiceService.API.Controllers.V1;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly ICustomerApiService _customerApiService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService service,
        IMapper mapper,
        ICacheService cacheService,
        ICustomerApiService customerApiService,
        ILogger<InvoicesController> logger)
    {
        _service = service;
        _mapper = mapper;
        _cacheService = cacheService;
        _customerApiService = customerApiService;
        _logger = logger;
    }

    
    // GET: api/invoices
    [HttpGet]
    [Authorize]
    public async Task<ActionResult>
        GetInvoices(
            [FromQuery] PaginationParameters parameters)
    {
        var result =
            await _service.GetPagedInvoices(parameters);

        var invoiceDtos =
            _mapper.Map<List<InvoiceDto>>(result.Invoices);

        var response =
            new PagedResponse<List<InvoiceDto>>(
                true,
                "Invoices fetched successfully",
                invoiceDtos,
                parameters.PageNumber,
                parameters.PageSize,
                result.TotalRecords
            );

        return Ok(response);
    }

    // GET: api/invoices/1
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<InvoiceDto>>
        GetInvoice(int id)
    {
        string cacheKey = $"invoice_{id}";

        var cachedInvoice =
            await _cacheService.GetData<InvoiceDto>(cacheKey);

        if (cachedInvoice != null)
        {
            _logger.LogInformation(
                $"Invoice {id} fetched from Redis Cache");

            return Ok(cachedInvoice);
        }

        _logger.LogInformation(
            $"Invoice {id} fetched from Database");

        var invoice =
            await _service.GetInvoiceById(id);

        if (invoice == null)
        {
            return NotFound(new
            {
                Message = $"Invoice with ID {id} not found."
            });
        }

        var invoiceDto =
            _mapper.Map<InvoiceDto>(invoice);

        await _cacheService.SetData(
            cacheKey,
            invoiceDto,
            TimeSpan.FromMinutes(5));

        return Ok(invoiceDto);
    }

    // POST: api/invoices
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InvoiceDto>>
        CreateInvoice(CreateInvoiceDto dto)
    {
        var customer =
            await _customerApiService
                .GetCustomer(dto.CustomerId);

        if (customer == null)
        {
            return BadRequest(new
            {
                Message = $"Customer {dto.CustomerId} not found."
            });
        }
        var invoice =
            _mapper.Map<Invoice>(dto);

        var createdInvoice =
            await _service.CreateInvoice(invoice);

        var response =
            _mapper.Map<InvoiceDto>(createdInvoice);

        await _cacheService.RemoveData("invoices_all");

        _logger.LogInformation(
            "Invoices cache invalidated after Create");
            
        return CreatedAtAction(
            nameof(GetInvoice),
            new { id = response.InvoiceId },
            response
        );
    }

    // PUT: api/invoices/1
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        UpdateInvoice(
            int id,
            UpdateInvoiceDto dto)
    {
        if (id != dto.InvoiceId)
        {
            return BadRequest(new
            {
                Message = "Invoice ID mismatch."
            });
        }

        var existingInvoice =
            await _service.GetInvoiceById(id);

        if (existingInvoice == null)
        {
            return NotFound(new
            {
                Message = $"Invoice with ID {id} not found."
            });
        }

        var invoice =
            _mapper.Map<Invoice>(dto);

        await _service.UpdateInvoice(invoice);

        await _cacheService.RemoveData("invoices_all");
        await _cacheService.RemoveData($"invoice_{id}");

        _logger.LogInformation(
            $"Cache invalidated for Invoice {id}");

        return Ok(new
        {
            Message = "Invoice updated successfully."
        });
    }

    // DELETE: api/invoices/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        DeleteInvoice(int id)
    {
        var existingInvoice =
            await _service.GetInvoiceById(id);

        if (existingInvoice == null)
        {
            return NotFound(new
            {
                Message = $"Invoice with ID {id} not found."
            });
        }

        await _service.DeleteInvoice(id);

        await _cacheService.RemoveData("invoices_all");
        await _cacheService.RemoveData($"invoice_{id}");

        _logger.LogInformation(
            $"Cache invalidated after deleting Invoice {id}");

        return Ok(new
        {
            Message = "Invoice deleted successfully."
        });
    }

    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("Invoices Controller Working");
    }
}