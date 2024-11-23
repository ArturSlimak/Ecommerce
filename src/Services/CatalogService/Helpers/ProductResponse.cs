using CatalogService.DTOs;

namespace CatalogService.Helpers;

public class ProductResponse
{
    public class GetIndex
    {
        public List<ProductDTO.Index> Products { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class Create
    {
        public string ProductId { get; set; } = null!;
    }
}
