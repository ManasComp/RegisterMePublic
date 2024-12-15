#region

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exceptions;

#endregion

namespace WebApi.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers = new()
    {
        { typeof(ValidationException), HandleValidationException },
        { typeof(NotFoundException), HandleNotFoundException },
        { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
        { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
        { typeof(InvalidDatabaseStateException), HandleInvalidDatabaseStateException },
        { typeof(BadHttpRequestException), HandleValidationException }
    };

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        Type exceptionType = exception.GetType();

        if (!_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, Task>? value))
        {
            return false;
        }

        await value.Invoke(httpContext, exception);
        return true;
    }

    private static async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        ValidationException exception = (ValidationException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }

    private static async Task HandleInvalidDatabaseStateException(HttpContext httpContext, Exception ex)
    {
        InvalidDatabaseStateException exception = (InvalidDatabaseStateException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The error in application occured.",
            Detail = exception.Message
        });
    }

    private static async Task HandleNotFoundException(HttpContext httpContext, Exception ex)
    {
        NotFoundException exception = (NotFoundException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        });
    }

    private static async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        });
    }

    private static async Task HandleForbiddenAccessException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        });
    }
}
