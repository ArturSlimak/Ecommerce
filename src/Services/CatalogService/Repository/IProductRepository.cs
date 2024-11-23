using CatalogService.Models.Product;
using MongoDB.Driver;

namespace CatalogService.Repository;

public interface IProductRepository
{
    Task<IList<Product>> GetProducts(FilterDefinition<Product> filter, SortDefinition<Product> sort, int page, int pageSize);
    Task InsertProduct(Product product);
}
