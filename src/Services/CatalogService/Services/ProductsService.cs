using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Exceptions;
using CatalogService.Helpers;
using CatalogService.Models.Product;
using CatalogService.Repositories;
using MongoDB.Driver;

namespace CatalogService.Services;

public class ProductsService : IProductsService
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
                ? Builders<Product>.Sort.Descending(p => p.Description)
                : Builders<Product>.Sort.Ascending(p => p.Description),
            _ => request.SortDescending
                ? Builders<Product>.Sort.Descending(p => p.Name)
                : Builders<Product>.Sort.Ascending(p => p.Name)
        };

        var products = await _productRepository.GetProductsAsync(filter, sort, request.Page, request.PageSize);

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
        return new ProductResponse.Create { ProductId = product.PublicId };
    }

    public async Task<ProductResponse.Mutate> MutateProductAsync(ProductRequest.Mutate request, string publicId)
    {
        var existingProduct = await _productRepository.GetProductByIdAsync(publicId);

        if (existingProduct == null)
        {
            throw new EntityNotFoundException($"{nameof(Product)} with id '{publicId}' not found.");
        }

        var updateDefinition = Builders<Product>.Update
      .Set(p => p.Name, request.Product.Name)
      .Set(p => p.Description, request.Product.Description);

        var filter = Builders<Product>.Filter.Eq(p => p.PublicId, publicId);

        await _productRepository.UpdateProductAsync(filter, updateDefinition);

        var updatedProduct = await _productRepository.GetProductByIdAsync(publicId);

        var response = new ProductResponse.Mutate
        {
            Product = _mapper.Map<ProductDTO.Index>(updatedProduct)
        };

        return response;

    }

    public async Task SoftDeleteProductAsync(string publicId)
    {
        var existingProduct = await _productRepository.GetProductByIdAsync(publicId);

        if (existingProduct == null)
        {
            throw new EntityNotFoundException($"{nameof(Product)} with id '{publicId}' not found.");
        }

        if (existingProduct.IsDeleted)
        {
            throw new EntityIsAlreadyDeleted($"{nameof(Product)} with id '{publicId}' is already deleted.");

        }

        var filter = Builders<Product>.Filter.Eq(p => p.PublicId, publicId);
        var update = Builders<Product>.Update.Set(p => p.IsDeleted, true).Set(p => p.DeletedAt, DateTime.UtcNow);

        await _productRepository.SoftDeleteProductAsync(filter, update);
    }
}
