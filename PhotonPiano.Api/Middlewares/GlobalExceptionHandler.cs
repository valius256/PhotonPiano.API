using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PhotonPiano.Shared.Exceptions;
using System.Net;

namespace PhotonPiano.Api.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/problem+json";
        _logger.LogError(exception, "Something went wrong while processing {RequestPath}, error: {ErrorMessage}",
            httpContext.Request.Path, exception.Message);

        var details = new ValidationProblemDetails
        {
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing request.",
            Type = "Internal Server Error",
            Errors = new Dictionary<string, string[]> { { "Exception", [exception.Message] } }
        };

        switch (exception)
        {
            case BadRequestException _:
                details.Status = (int)HttpStatusCode.BadRequest;
                details.Type = "Bad Request";
                break;
            case NotFoundException _:
                details.Status = (int)HttpStatusCode.NotFound;
                details.Type = "Not Found";
                break;
            case UnauthorizedException _:
                details.Status = (int)HttpStatusCode.Unauthorized;
                details.Type = "Unauthorized";
                break;
            case ForbiddenMethodException _:
                details.Status = (int)HttpStatusCode.Forbidden;
                details.Type = "Forbidden";
                break;
            case ConflictException _:
                details.Status = (int)HttpStatusCode.Conflict;
                details.Type = "Conflict";
                break;
            case PaymentRequiredException _:
                details.Status = (int)HttpStatusCode.PaymentRequired;
                details.Type = "PaymentRequired";
                break;
            case CustomException customException:
                details.Status = customException.Code;
                details.Type = "Other";
                break;
            case IllegalArgumentException iillegalArgumentException:
                details.Status = (int)HttpStatusCode.BadRequest;
                details.Type = "Illegal Argument";
                break;
        }

        var response = JsonConvert.SerializeObject(details);
        httpContext.Response.StatusCode = details.Status.Value;

        await httpContext.Response.WriteAsync(response, cancellationToken);

        return true;
    }
}