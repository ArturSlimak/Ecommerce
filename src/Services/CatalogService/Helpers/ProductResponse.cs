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
    public class GetDetails
    {
        public ProductDTO.Details Product { get; set; } = new();

    }

    public class Create
    {
        public string PublicId { get; set; } = null!;
    }

    public class Mutate
    {
        public ProductDTO.Index Product { get; set; } = null!;
    }

}
