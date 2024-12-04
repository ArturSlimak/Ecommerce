using InventoryService.DTOs;

namespace InventoryService.Helpers;

public class ProductResponse
{
    public class GetIndex
    {
        public List<ProductDTO.Index> Products { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
    }


}
