using Serilog.Context;

namespace Gateway.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string headerName = "X-Correlation-ID";

        if (!context.Request.Headers.TryGetValue(
                headerName,
                out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers.Append(
            headerName,
            correlationId.ToString());

        using (LogContext.PushProperty(
            "CorrelationId",
            correlationId.ToString()))
        {
            await _next(context);
        }
    }
}