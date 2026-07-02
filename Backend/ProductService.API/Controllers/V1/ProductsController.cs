using AutoMapper;
using Asp.Versioning;
using ProductService.API.Caching;
using ProductService.API.DTOs;
using ProductService.API.Interfaces;
using ProductService.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Responses;
using ProductService.API.Helpers;


namespace ProductService.API.Controllers.V1;

/// <summary>
/// APIs for managing products.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService service,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<ProductsController> logger)
    {
        _service = service;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    // GET: api/products

    /// <summary>
    /// Retrieves all products with pagination, search, sorting and filtering.
    /// </summary>
    /// <param name="paginationParameters">
    /// Pagination, search, sorting and filtering parameters.
    /// </param>
    /// <returns>Returns a paginated list of products.</returns>
    
    [ProducesResponseType(typeof(PagedResponse<List<ProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResponse<List<ProductDto>>>> GetProducts( [FromQuery] PaginationParameters paginationParameters)
    {

    var version =
        await _cacheService.GetCacheVersionAsync();

    _logger.LogInformation(
    $"Current cache version = {version}");



    string cacheKey =
        $"products_v{version}_" +
        $"{paginationParameters.PageNumber}_" +
        $"{paginationParameters.PageSize}_" +
        $"{paginationParameters.Search}_" +
        $"{paginationParameters.SortBy}_" +
        $"{paginationParameters.SortOrder}_" +
        $"{paginationParameters.MinPrice}_" +
        $"{paginationParameters.MaxPrice}";

        _logger.LogInformation(
        $"Cache key = {cacheKey}");

        var totalRecords =
        await _service.GetTotalCount(paginationParameters);
   
        var cachedProducts =
            await _cacheService.GetData<List<ProductDto>>(cacheKey);
            

        if (cachedProducts != null)
        {
            _logger.LogInformation(
                "Products fetched from Redis Cache");

            return Ok(
                new PagedResponse<List<ProductDto>>(
                    true,
                    "Products fetched successfully from cache.",
                    cachedProducts,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize,
                    totalRecords
                )
            );
        }

        _logger.LogInformation(
            "Products fetched from Database");

        var products =
            await _service.GetAllProducts(paginationParameters);

        var productDtos =
            _mapper.Map<List<ProductDto>>(products);

        await _cacheService.SetData(
            cacheKey,
            productDtos,
            TimeSpan.FromMinutes(5));

        return Ok(
            new PagedResponse<List<ProductDto>>(
                true,
                "Products fetched successfully.",
                productDtos,
                paginationParameters.PageNumber,
                paginationParameters.PageSize,
                totalRecords
            )
        );
    }

    // GET: api/products/1
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductDto>>>
    GetProduct(int id)
    {
        string cacheKey = $"product_{id}";

        var cachedProduct =
            await _cacheService.GetData<ProductDto>(cacheKey);

        if (cachedProduct != null)
        {
            _logger.LogInformation(
                $"Product {id} fetched from Redis Cache");

            return Ok(
                new ApiResponse<ProductDto>(
                    true,
                    "Product fetched successfully from cache.",
                    cachedProduct
                )
            );
        }

        _logger.LogInformation(
            $"Product {id} fetched from Database");

        var product =
            await _service.GetProductById(id);

        if (product == null)
        {
            return NotFound(
                new ApiResponse<ProductDto>(
                    false,
                    $"Product with ID {id} not found.",
                    null
                )
            );
        }

        var productDto =
            _mapper.Map<ProductDto>(product);

        await _cacheService.SetData(
            cacheKey,
            productDto,
            TimeSpan.FromMinutes(5));

        return Ok(
            new ApiResponse<ProductDto>(
                true,
                "Product fetched successfully.",
                productDto
            )
        );
    }

    [AllowAnonymous]
    [HttpGet("internal/{id}")]
    public async Task<ActionResult<ProductDto>>
    GetProductInternal(int id)
    {
        var product =
            await _service.GetProductById(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(
            _mapper.Map<ProductDto>(product));
    }
    // POST: api/products
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductDto>>>
        CreateProduct(CreateProductDto dto)
    {
        Console.WriteLine("========== DTO ==========");
        Console.WriteLine($"Name = {dto.Name}");
        Console.WriteLine($"Price = {dto.Price}");
        Console.WriteLine($"Quantity = {dto.Quantity}");

        var product = _mapper.Map<Product>(dto);

        Console.WriteLine("========== ENTITY ==========");
        Console.WriteLine($"Name = {product.Name}");
        Console.WriteLine($"Price = {product.Price}");
        Console.WriteLine($"Quantity = {product.Quantity}");

        var createdProduct =
            await _service.CreateProduct(product);

        Console.WriteLine("========== SAVED ==========");
        Console.WriteLine($"Id = {createdProduct.ProductId}");
        Console.WriteLine($"Name = {createdProduct.Name}");
        Console.WriteLine($"Price = {createdProduct.Price}");
        Console.WriteLine($"Quantity = {createdProduct.Quantity}");

        var response =
            _mapper.Map<ProductDto>(createdProduct);

        await _cacheService.IncrementCacheVersionAsync();

        _logger.LogInformation(
            "Products cache Version Incremented after Create");

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = response.ProductId },
            new ApiResponse<ProductDto>(
                true,
                "Product created successfully.",
                response
            )
        );
    }

    // PUT: api/products/1
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        UpdateProduct(int id, UpdateProductDto dto)
    {
        if (id != dto.ProductId)
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Product ID mismatch.",
                    null
                )
            );
        }

        var existingProduct =
            await _service.GetProductById(id);

        if (existingProduct == null)
        {
            return NotFound(
                new ApiResponse<object>(
                    false,
                    $"Product with ID {id} not found.",
                    null
                )
            );
        }

        existingProduct.Name = dto.Name;
        existingProduct.Price = dto.Price;
        existingProduct.Quantity = dto.Quantity;

        await _service.UpdateProduct(existingProduct);

        await _cacheService.IncrementCacheVersionAsync();
        await _cacheService.RemoveData($"product_{id}");

        _logger.LogInformation(
            $"Cache version incremented after updated Product {id}");

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Product updated successfully.",
                    null
                )
            );
    }

    // DELETE: api/products/1
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        DeleteProduct(int id)
    {
        var existingProduct =
            await _service.GetProductById(id);

        if (existingProduct == null)
        {
            return NotFound(
                new ApiResponse<object>(
                    false,
                    $"Product with ID {id} not found.",
                    null
                )
            );
        }

        await _service.DeleteProduct(id);

        await _cacheService.IncrementCacheVersionAsync();
        await _cacheService.RemoveData($"product_{id}");

        _logger.LogInformation(
            $"Cache version incremented after deleting Product {id}");

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Product deleted successfully.",
                    null
                )
            );
    }

    [AllowAnonymous]
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Product Service Working");
    }
}