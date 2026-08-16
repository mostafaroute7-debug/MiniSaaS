using MiniSaaS.Application.Common.Interfaces;
using MiniSaaS.Infrastructure.MultiTenancy;


namespace MiniSaaS.API.Middleware;

public class TenantMiddleware
{
    private const string TenantHeader = "X-Tenant-Id";

    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContextAccessor tenantContextAccessor,
        ITenantReader tenantReader)
    {
        var endpoint = context.GetEndpoint();

        var requiresTenant =
            endpoint?.Metadata.GetMetadata<TenantRequiredAttribute>()
            is not null;

        if (!requiresTenant)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(
                TenantHeader,
                out var tenantHeader))
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Tenant header is required.");

            return;
        }

        if (!int.TryParse(
                tenantHeader.ToString(),
                out var tenantId) ||
            tenantId <= 0)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Tenant ID must be a valid positive integer.");

            return;
        }

        var tenantExists =
            await tenantReader.ExistsAndIsActiveAsync(
                tenantId,
                context.RequestAborted);

        if (!tenantExists)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status404NotFound,
                "Tenant was not found or is inactive.");

            return;
        }

        tenantContextAccessor.SetTenant(tenantId);

        await _next(context);
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode,
            message
        });
    }
}
