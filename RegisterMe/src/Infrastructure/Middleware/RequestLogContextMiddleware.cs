#region

using Microsoft.AspNetCore.Http;
using Serilog.Context;

#endregion

namespace RegisterMe.Infrastructure.Middleware;

public class RequestLogContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        {
            await next(context);
        }
    }
}
