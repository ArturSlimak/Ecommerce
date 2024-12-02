using CatalogService.Helpers;

namespace CatalogService.Services;

public interface IProductsService
{
    Task<ProductResponse.Create> CreateAsync(ProductRequest.Create request);
    Task SoftDeleteProductAsync(string id);
    Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request);
    Task<ProductResponse.Mutate> MutateProductAsync(ProductRequest.Mutate request, string id);
}