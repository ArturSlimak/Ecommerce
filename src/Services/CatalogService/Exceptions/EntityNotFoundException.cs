using System.Net;

namespace CatalogService.Exceptions;

public class EntityNotFoundException : BaseException
{
    public EntityNotFoundException(string message)
    : base(message, HttpStatusCode.NotFound)
    {
    }
}
