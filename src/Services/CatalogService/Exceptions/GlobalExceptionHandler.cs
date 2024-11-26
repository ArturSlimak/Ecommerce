using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StackExchange.Redis;

namespace CatalogService.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["method"] = httpContext.Request.Method;

        switch (exception)
        {
            case BaseException e:
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = e.Message;
                break;

            case ArgumentNullException e:
                httpContext.Response.StatusCode = 400;
                problemDetails.Title = "Missing required argument.";
                break;

            case UnauthorizedAccessException e:
                httpContext.Response.StatusCode = 401;
                problemDetails.Title = "Unauthorized access.";
                break;

            case MongoConnectionException e:
                httpContext.Response.StatusCode = 503;
                problemDetails.Title = "Database connection error";
                break;

            case MongoCommandException e:
                httpContext.Response.StatusCode = 400;
                problemDetails.Title = "Database command failed";
                break;

            case MongoException mongoException:
                httpContext.Response.StatusCode = 500;
                problemDetails.Title = "Database error occurred";
                break;

            case RedisConnectionException e:
                httpContext.Response.StatusCode = 503;
                problemDetails.Title = "Redis connection error";
                break;

            case RedisTimeoutException e:
                httpContext.Response.StatusCode = 408;
                problemDetails.Title = "Redis request timed out";
                break;

            case RedisException e:
                httpContext.Response.StatusCode = 500;
                problemDetails.Title = "Redis error occurred";
                break;

            default:
                httpContext.Response.StatusCode = 500;
                problemDetails.Title = "An unexpected error occurred.";
                break;
        }
        logger.LogError("{ProblemDetailsTitle}: {Message}", problemDetails.Title, exception.Message);
        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
