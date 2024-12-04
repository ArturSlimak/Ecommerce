using InventoryService.Models.Product;

namespace InventoryService.Repositories;

public interface IProductRepository
{
    Task<IList<Product>> GetProductsAsync();


}
