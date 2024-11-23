using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Helpers;
using CatalogService.Models;
using CatalogService.Repository;
using MongoDB.Driver;

namespace CatalogService.Services;

public class ProductsService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductsService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request)
    {
        //TODO
        var filter = Builders<Product>.Filter.Empty;
        var sort = Builders<Product>.Sort.Ascending(p => p.Name);

        var products = await _productRepository.GetProducts(filter, sort, request.Page, request.PageSize);
        var productDTOs = _mapper.Map<List<ProductDTO.Index>>(products);
        var pageSize = productDTOs.Count;
        var response = new ProductResponse.GetIndex
        {
            Products = productDTOs,
            PageSize = pageSize,
            Page = request.Page,
        };
        return response;
    }

    public async Task<ProductResponse.Create> CreateAsync(ProductRequest.Create request)
    {
        var product = _mapper.Map<Product>(request.Product);
        await _productRepository.InsertProduct(product);
        return new ProductResponse.Create { ProductId = product.Id };
    }

}
