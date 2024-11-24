using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace CatalogService.Exceptions;

public class ValidationFailException : BaseException
{
    public ValidationFailException(ModelStateDictionary modelState)
       : base(BuildErrorMessage(modelState), HttpStatusCode.BadRequest)
    {
    }

    private static string BuildErrorMessage(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(ms => ms.Value?.Errors.Count > 0)
            .Select(ms => new
            {
                Field = ms.Key,
                Errors = ms.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
            })
            .ToArray();

        return string.Join("; ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors)}"));
    }
}
