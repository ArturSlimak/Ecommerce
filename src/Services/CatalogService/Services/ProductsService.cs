using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Exceptions;
using CatalogService.Extensions;
using CatalogService.Helpers;
using CatalogService.Models.Product;
using CatalogService.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace CatalogService.Services;

public class ProductsService : IProductsService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public ProductsService(IProductRepository productRepository, IMapper mapper, IDistributedCache cache)
    {
        _productRepository = productRepository;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request)
    {
        var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        //TODO
        var filter = Builders<Product>.Filter.Empty;

        SortField sortField = Enum.TryParse<SortField>(request.SortBy, true, out var parsedSortField)
                    ? parsedSortField
                    : SortField.Name;

        var sort = sortField switch
        {
            SortField.Name => request.SortDescending
                ? Builders<Product>.Sort.Descending(p => p.Name)
                : Builders<Product>.Sort.Ascending(p => p.Name),
            SortField.Price => request.SortDescending
                ? Builders<Product>.Sort.Descending(p => p.Price)
                : Builders<Product>.Sort.Ascending(p => p.Price),
            _ => request.SortDescending
                ? Builders<Product>.Sort.Descending(p => p.Name)
                : Builders<Product>.Sort.Ascending(p => p.Name)
        };

        var cacheKey = $"products_page_{request.Page}_size_{request.PageSize}";


        var products = await _cache.GetOrSetAsync(
           cacheKey,
           async () =>
           {
               return await _productRepository.GetProductsAsync(filter, sort, request.Page, request.PageSize); ;
           },
           cacheOptions)!;

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
        await _productRepository.InsertProductAsync(product);
        return new ProductResponse.Create { ProductId = product.Id };
    }

    public async Task<ProductResponse.Mutate> MutateProductAsync(ProductRequest.Mutate request, string id)
    {
        var existingProduct = await _productRepository.GetProductByIdAsync(id);

        if (existingProduct == null)
        {
            throw new EntityNotFoundException($"{nameof(Product)} with id '{id}' not found.");
        }
        var updatedProduct = _mapper.Map(request.Product, existingProduct);

        await _productRepository.UpdateProductAsync(updatedProduct);

        var response = new ProductResponse.Mutate
        {
            Product = _mapper.Map<ProductDTO.Index>(updatedProduct)
        };

        return response;

    }
}
