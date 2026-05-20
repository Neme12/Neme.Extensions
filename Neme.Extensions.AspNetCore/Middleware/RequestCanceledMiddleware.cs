using Microsoft.AspNetCore.Http;

namespace Neme.Extensions.AspNetCore.Middleware;

internal sealed class RequestCanceledMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException e) when (e.CancellationToken == context.RequestAborted)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode =
#if NET8_0_OR_GREATER
                    StatusCodes.Status499ClientClosedRequest;
#else
                    499;
#endif
            }

            await context.Response.CompleteAsync();
        }
    }
}
