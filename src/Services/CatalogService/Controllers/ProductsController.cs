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
    public async Task<ProductResponse.GetIndex> Get([FromQuery] ProductRequest.Index request)
    {
        var response = await _productsService.GetAsync(request);
        _logger.LogDebug("Retrieved all products. Products: {@Products}", response.Products);
        return response;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductRequest.Create request)
    {
        var response = await _productsService.CreateAsync(request);
        _logger.LogDebug("Added a new product {@Product}", response);
        return CreatedAtAction(nameof(Get), new { id = response.ProductId });
    }

}
