using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Helpers;
using CatalogService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogService.Services;

public class ProductsService
{
    private readonly IMongoCollection<Product> _productsCollection;
    private readonly IMapper _mapper;

    public ProductsService(IOptions<CatalogDBSettings> catalogDBSettings, IMapper mapper)
    {
        MongoClient mongoClient = new(catalogDBSettings.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(catalogDBSettings.Value.DatabaseName);
        _productsCollection = mongoDatabase.GetCollection<Product>(catalogDBSettings.Value.Collections.Products);
        _mapper = mapper;
    }

    public async Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request)
    {
        var products = await _productsCollection.Find(_ => true)
            .Skip((request.Page - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync();
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
        await _productsCollection.InsertOneAsync(product);
        return new ProductResponse.Create { ProductId = product.Id };
    }

}
