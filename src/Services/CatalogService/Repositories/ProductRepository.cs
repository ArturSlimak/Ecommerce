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

    public async Task<Product> GetProductByIdAsync(string id)
    {
        return await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
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

    public async Task UpdateProductAsync(Product updatedProduct)
    {
        await _productsCollection.UpdateOneAsync(p => p.Id == updatedProduct.Id, Builders<Product>.Update
            .Set(p => p.Name, updatedProduct.Name)
            .Set(p => p.Price, updatedProduct.Price));
    }
}
