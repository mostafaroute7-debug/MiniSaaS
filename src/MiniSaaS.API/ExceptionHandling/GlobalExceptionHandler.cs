using Microsoft.AspNetCore.Diagnostics;
using MiniSaaS.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace MiniSaaS.API.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
    {
        _logger.LogError(exception,"Unhandled exception occurred. TraceId: {TraceId}",httpContext.TraceIdentifier);

        var response = ResultDto<object>.Failure( "An unexpected error occurred.", ErrorCode.InternalServerError);

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        httpContext.Response.ContentType ="application/json";

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response),cancellationToken);

        return true;
    }
}