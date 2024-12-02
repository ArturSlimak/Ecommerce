using System.Net;

namespace CatalogService.Exceptions;

public class EntityIsAlreadyDeleted : BaseException
{
    public EntityIsAlreadyDeleted(string message)
   : base(message, HttpStatusCode.Conflict)
    {
    }
}
