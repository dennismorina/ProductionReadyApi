using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductionReadyApi.Application.Common.Exceptions;
using ProductionReadyApi.Domain.Exceptions;

namespace ProductionReadyApi.Api.ErrorHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            RequestValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                exception.Message),
            DomainException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                exception.Message),
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred.")
        };

        if (statusCode >= 500)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            logger.LogInformation(
                "Request {Method} {Path} failed with status {StatusCode}: {Message}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                statusCode,
                exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}",
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (exception is RequestValidationException validationException)
        {
            problem.Extensions["errors"] = validationException.Errors;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
