using CatalogService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogService.Repository;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductRepository(IMongoDatabase database, IOptions<CatalogDBSettings> catalogDBSettings)
    {
        _productsCollection = database.GetCollection<Product>(catalogDBSettings.Value.Collections.Products);
    }

    public async Task<IList<Product>> GetProducts(FilterDefinition<Product> filter, SortDefinition<Product> sort, int page, int pageSize)
    {
        var items = await _productsCollection.Find(filter)
                                              .Sort(sort)
                                              .Skip((page - 1) * pageSize)
                                              .Limit(pageSize)
                                              .ToListAsync();
        return items;
    }

    public async Task InsertProduct(Product product)
    {
        await _productsCollection.InsertOneAsync(product);
    }
}
