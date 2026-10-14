using Microsoft.AspNetCore.Http;

namespace Neme.Extensions.AspNetCore.Middleware;

public sealed class XHtmlMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var nextFailed = false;

        context.Response.OnStarting(() =>
        {
            if (!nextFailed)
            {
                var contentType = context.Response.ContentType;

                if (!contentType.IsNullOrEmpty())
                    context.Response.ContentType = ConvertContentType(contentType);
            }

            return Task.CompletedTask;
        });

        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch
        {
            nextFailed = true;
            throw;
        }
    }

    private static string ConvertContentType(string contentType)
    {
        var endIndex = contentType.IndexOf(';');
        if (endIndex == -1)
            endIndex = contentType.Length;

        var typePart = contentType[..endIndex];
        var parameterPart = contentType[endIndex..];

        if (typePart.Equals("text/html", StringComparison.OrdinalIgnoreCase))
            typePart = "application/xhtml+xml";

        return typePart + parameterPart;
    }
}
