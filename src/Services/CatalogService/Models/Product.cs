using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogService.Models;

public class Product : Entity
{

    [BsonElement("name")]
    public string ProductName { get; set; } = null!;

    [BsonElement("price")]
    public decimal Price { get; set; }
}
