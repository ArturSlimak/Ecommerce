using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class InventoriesController : ControllerBase
    {
    }
}
