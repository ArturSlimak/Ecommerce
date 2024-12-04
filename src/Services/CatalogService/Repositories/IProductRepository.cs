using CatalogService.Models.Product;
using MongoDB.Driver;

namespace CatalogService.Repositories;

public interface IProductRepository
{
    Task<IList<Product>> GetProductsAsync(FilterDefinition<Product> filter, SortDefinition<Product> sort, int page, int pageSize);
    Task<Product> GetProductByIdAsync(string publicId);
    Task InsertProductAsync(Product product);
    Task UpdateProductAsync(FilterDefinition<Product> filter, UpdateDefinition<Product> update);
    Task SoftDeleteProductAsync(FilterDefinition<Product> filter, UpdateDefinition<Product> update);
}
