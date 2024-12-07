using Asp.Versioning;
using CatalogService.Helpers;
using CatalogService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{

    private readonly ILogger<ProductsController> _logger;
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService, ILogger<ProductsController> logger)
    {
        _productsService = productsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ProductResponse.GetIndex>> GetProducts([FromQuery] ProductRequest.Index request)
    {
        var response = await _productsService.GetAsync(request);
        _logger.LogDebug("Retrieved all products. Products: {@Products}", response.Products);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse.GetDetails>> GetProductById(string id)
    {
        var response = await _productsService.GetByIdAsync(id);
        _logger.LogDebug("Retrieved product with publicId {@Id}. Product: {@Product}", id, response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequest.Create request)
    {
        var response = await _productsService.CreateAsync(request);
        _logger.LogDebug("Added a new product {@Product}", response);
        return CreatedAtAction(nameof(GetProducts), new { id = response.PublicId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> MutateProduct([FromBody] ProductRequest.Mutate request, string id)
    {
        var response = await _productsService.MutateAsync(request, id);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        await _productsService.SoftDeleteProductAsync(id);
        return Ok();
    }



}