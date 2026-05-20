using Microsoft.AspNetCore.Builder;
using Neme.Extensions.Contracts;

namespace Neme.Extensions.AspNetCore.Middleware;

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder builder)
    {
        public IApplicationBuilder UseRequestCanceled()
        {
            Require.ArgumentNotNull(builder);

            builder.UseMiddleware<RequestCanceledMiddleware>();
            return builder;
        }

        public IApplicationBuilder UseXHtml()
        {
            Require.ArgumentNotNull(builder);

            builder.UseMiddleware<XHtmlMiddleware>();
            return builder;
        }
    }
}
