using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Helpers;
using CatalogService.Models.Product;
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
