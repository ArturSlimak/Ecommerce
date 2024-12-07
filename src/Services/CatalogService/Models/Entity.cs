using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogService.Models;

public class Entity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("public_id")]
    public string PublicId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("deleted")]
    public bool IsDeleted { get; set; } = false;
    [BsonElement("deleted_at")]
    [BsonIgnoreIfNull]
    public DateTime? DeletedAt { get; set; }
}
