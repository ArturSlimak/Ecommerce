using InventoryService.Data;
using InventoryService.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories;

public class ProductRepository : IProductRepository
{

    private readonly InventoryDbContext _DbContext;

    public ProductRepository(InventoryDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<IList<Product>> GetProductsAsync()
    {
        return await _DbContext.Products.ToListAsync();
    }
}
