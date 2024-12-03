using CatalogService.Controllers;
using CatalogService.DTOs;
using CatalogService.Helpers;
using CatalogService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CatalogService.UnitTests;

public class ProductsControllerTests
{
    private Mock<IProductsService> _mockProductsService;
    private ProductsController _controller;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;

    public ProductsControllerTests()
    {
        _mockProductsService = new Mock<IProductsService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockProductsService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Get_ValidRequest_ReturnsOkResultWithProducts()
    {
        var request = new ProductRequest.Index();
        var productResponse = new ProductResponse.GetIndex
        {
            Products = new List<ProductDTO.Index>
        {
            new ProductDTO.Index { Id = "1", Name = "Product1", Description = 100 },
            new ProductDTO.Index { Id = "2", Name = "Product2", Description = 200 }
        }
        };

        _mockProductsService.Setup(service => service.GetAsync(request)).ReturnsAsync(productResponse);

        var actionResult = await _controller.Get(request);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<ProductResponse.GetIndex>(okResult.Value);

        Assert.Equal(2, response.Products.Count);
    }


}
