using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Extensions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["method"] = httpContext.Request.Method;

        if (exception is BaseException e)
        {
            httpContext.Response.StatusCode = (int)e.StatusCode;
            problemDetails.Title = e.Message;
        }
        else
        {
            problemDetails.Title = "An error occurred";

        }
        logger.LogError("{ProblemDetailsTitle}", problemDetails.Title);
        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
