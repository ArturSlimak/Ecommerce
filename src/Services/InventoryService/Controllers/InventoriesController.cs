using Asp.Versioning;
using InventoryService.Helpers;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class InventoriesController : ControllerBase
{

    private readonly ILogger<InventoriesController> _logger;
    private readonly IProductsService _productsService;

    public InventoriesController(ILogger<InventoriesController> logger, IProductsService productsService)
    {
        _logger = logger;
        _productsService = productsService;
    }

    [HttpGet]
    public async Task<ActionResult<ProductResponse.GetIndex>> GetProducts([FromQuery] ProductRequest.Index request)
    {
        var response = await _productsService.GetAsync(request);
        _logger.LogDebug("Retrieved all products. Products: {@Products}", response.Products);
        return Ok(response);
    }


    /*  [HttpPost]
      public async Task<IActionResult> CreateProduct([FromBody] ProductRequest.Create request)
      {
          var response = await _productsService.CreateAsync(request);
          _logger.LogDebug("Added a new product {@Product}", response);
          return CreatedAtAction(nameof(GetProducts), new { id = response.ProductId });
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> MutateProduct([FromBody] ProductRequest.Mutate request, string id)
      {
          var response = await _productsService.MutateProductAsync(request, id);
          return Ok(response);
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteProduct(string id)
      {
          await _productsService.SoftDeleteProductAsync(id);
          return Ok();
      }*/

}
