using CatalogService.Models.Product;
using MongoDB.Driver;

namespace CatalogService.Repositories;

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(string id);
    Task<IList<Product>> GetProductsAsync(FilterDefinition<Product> filter, SortDefinition<Product> sort, int page, int pageSize);
    Task InsertProductAsync(Product product);
    Task UpdateProductAsync(Product updatedProduct);
}
