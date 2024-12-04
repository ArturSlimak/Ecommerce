namespace InventoryService.Models.Product;

public class Product : Entity
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
