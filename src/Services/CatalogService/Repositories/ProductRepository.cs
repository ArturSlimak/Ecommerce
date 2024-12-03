using CatalogService.Models;
using CatalogService.Models.Product;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogService.Repositories;


public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductRepository(IMongoDatabase database, IOptions<CatalogDBSettings> catalogDBSettings)
    {
        _productsCollection = database.GetCollection<Product>(catalogDBSettings.Value.Collections.Products);
    }

    public async Task<Product> GetProductByIdAsync(string publicId)
    {
        return await _productsCollection.Find(p => p.PublicId == publicId).FirstOrDefaultAsync();
    }

    public async Task<IList<Product>> GetProductsAsync(FilterDefinition<Product> filter, SortDefinition<Product> sort, int page, int pageSize)
    {
        var items = await _productsCollection.Find(filter)
                                              .Sort(sort)
                                              .Skip((page - 1) * pageSize)
                                              .Limit(pageSize)
                                              .ToListAsync();
        return items;
    }

    public async Task InsertProductAsync(Product product)
    {
        await _productsCollection.InsertOneAsync(product);
    }

    public async Task SoftDeleteProductAsync(FilterDefinition<Product> filter, UpdateDefinition<Product> update)
    {
        await UpdateDocumentAsync(filter, update);
    }

    public async Task UpdateProductAsync(FilterDefinition<Product> filter, UpdateDefinition<Product> update)
    {
        await UpdateDocumentAsync(filter, update);
    }

    private async Task UpdateDocumentAsync(FilterDefinition<Product> filter, UpdateDefinition<Product> update)
    {
        var extendedUpdate = update.Set(p => p.UpdatedAt, DateTime.UtcNow);
        await _productsCollection.UpdateOneAsync(filter, extendedUpdate);
    }
}
