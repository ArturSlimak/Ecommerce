using CatalogService.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CatalogService.Filters;

public class ModelValidationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            throw new ValidationFailException(context.ModelState);
    }
}
