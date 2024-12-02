using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Net;

namespace CatalogService.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["method"] = httpContext.Request.Method;
        problemDetails.Detail = exception.Message;


        switch (exception)
        {
            // Custom
            case EntityIsAlreadyDeleted e:
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = "Entity is already deleted";
                break;

            case EntityNotFoundException e:
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = "Entity not found";
                break;

            case ValidationFailException e:
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = "Validation fail";
                break;

            case BaseException e:
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = "Something went wrong";
                break;


            // Not Custom
            case ArgumentNullException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Missing required argument.";
                break;

            case UnauthorizedAccessException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Unauthorized access.";
                break;

            case MongoExecutionTimeoutException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                problemDetails.Title = "Database timeout error";
                break;

            case MongoConnectionException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                problemDetails.Title = "Database connection error";
                break;

            case MongoCommandException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Database command failed";
                break;

            case MongoException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Database error occurred";
                break;

            case RedisConnectionException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                problemDetails.Title = "Redis connection error";
                break;

            case RedisTimeoutException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                problemDetails.Title = "Redis request timed out";
                break;

            case RedisException _:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Redis error occurred";
                break;

            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "An unexpected error occurred.";
                break;
        }
        problemDetails.Status = httpContext.Response.StatusCode;

        logger.LogError("{StatusCode}/{Title}: {Detail}", problemDetails.Status, problemDetails.Title, problemDetails.Detail);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
