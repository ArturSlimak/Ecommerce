namespace CatalogService.Models;

public class CatalogDBSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public MongoCollections Collections { get; set; } = null!;
}

public class MongoCollections
{
    public string Products { get; set; } = null!;
    public string Categories { get; set; } = null!;
}