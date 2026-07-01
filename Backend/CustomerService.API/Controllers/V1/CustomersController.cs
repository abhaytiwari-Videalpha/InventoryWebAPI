using AutoMapper;
using CustomerService.API.DTOs;
using CustomerService.API.Interfaces;
using CustomerService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomerService.API.Caching;
using Asp.Versioning;
using CustomerService.API.Helpers;
using CustomerService.API.Responses;


namespace CustomerService.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly IMapper _mapper;

    private readonly ICacheService _cacheService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerService service,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<CustomersController> logger)
    {
        _service = service;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<List<CustomerDto>>>>
    GetCustomers(
        [FromQuery] PaginationParameters paginationParameters)
    {
        string cacheKey =
            $"customers_" +
            $"{paginationParameters.PageNumber}_" +
            $"{paginationParameters.PageSize}_" +
            $"{paginationParameters.Search}_" +
            $"{paginationParameters.SortBy}_" +
            $"{paginationParameters.SortOrder}";

        var cachedCustomers =
            await _cacheService.GetData<List<CustomerDto>>(cacheKey);

        if (cachedCustomers != null)
        {
            _logger.LogInformation(
                "Customers fetched from Redis Cache");

            var cachedResponse =
                new PagedResponse<List<CustomerDto>>(
                    true,
                    "Customers fetched successfully",
                    cachedCustomers,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize,
                    cachedCustomers.Count);

            return Ok(cachedResponse);
        }

        _logger.LogInformation(
            "Customers fetched from Database");

        var result =
            await _service.GetPagedCustomers(
                paginationParameters);

        var customerDtos =
            _mapper.Map<List<CustomerDto>>(
                result.Customers);

        await _cacheService.SetData(
            cacheKey,
            customerDtos,
            TimeSpan.FromMinutes(5));
            
            var response =
                new PagedResponse<List<CustomerDto>>(
                    true,
                    "Customers fetched successfully",
                    customerDtos,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize,
                    result.TotalRecords);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>>
        GetCustomer(int id)
    {
        string cacheKey = $"customer_{id}";

        var cachedCustomer =
            await _cacheService.GetData<CustomerDto>(cacheKey);

        if (cachedCustomer != null)
        {
            _logger.LogInformation(
                $"Customer {id} fetched from Redis Cache");

            return Ok(cachedCustomer);
        }

        _logger.LogInformation(
            $"Customer {id} fetched from Database");

        var customer =
            await _service.GetCustomerById(id);

        if (customer == null)
            return NotFound();

        var customerDto =
            _mapper.Map<CustomerDto>(customer);

        await _cacheService.SetData(
            cacheKey,
            customerDto,
            TimeSpan.FromMinutes(5));

        return Ok(customerDto);
    }

    [AllowAnonymous]
    [HttpGet("internal/{id}")]
    public async Task<ActionResult<CustomerDto>>
    GetCustomerInternal(int id)
    {
        var customer =
            await _service.GetCustomerById(id);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(
            _mapper.Map<CustomerDto>(customer));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CustomerDto>>
        CreateCustomer(CreateCustomerDto dto)
    {
        var customer =
            _mapper.Map<Customer>(dto);

        var createdCustomer =
            await _service.CreateCustomer(customer);

        await _cacheService.RemoveData("customers_all");

        _logger.LogInformation(
            "Customers cache invalidated after Create");

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = createdCustomer.CustomerId },
            _mapper.Map<CustomerDto>(createdCustomer)
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        UpdateCustomer(
            int id,
            UpdateCustomerDto dto)
    {
        if (id != dto.CustomerId)
            return BadRequest();

        var existingCustomer =
            await _service.GetCustomerById(id);

        if (existingCustomer == null)
            return NotFound();

        var customer =
            _mapper.Map<Customer>(dto);

        await _service.UpdateCustomer(customer);

        await _cacheService.RemoveData("customers_all");
        await _cacheService.RemoveData($"customer_{id}");

        _logger.LogInformation(
            $"Cache invalidated for Customer {id}");

        return Ok(new
        {
            Message = "Customer updated successfully"
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        DeleteCustomer(int id)
    {
        var customer =
            await _service.GetCustomerById(id);

        if (customer == null)
            return NotFound();

        await _service.DeleteCustomer(id);
        await _cacheService.RemoveData("customers_all");
        await _cacheService.RemoveData($"customer_{id}");

        _logger.LogInformation(
            $"Cache invalidated after deleting Customer {id}");

        return Ok(new
        {
            Message = "Customer deleted successfully"
        });
    }
    [HttpGet("ping")]
    [AllowAnonymous]
    public IActionResult Ping()
    {
        return Ok("Customers Controller Working");
    }
}