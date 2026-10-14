using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Neme.Extensions.AspNetCore.Middleware;
using Neme.Extensions.Contracts;

namespace Neme.Extensions.MicrosoftExtensions.Middleware;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRequestCanceled()
        {
            Require.ArgumentNotNull(services);

            services.TryAddSingleton<RequestCanceledMiddleware>();
            return services;
        }

        public IServiceCollection AddXHtml()
        {
            Require.ArgumentNotNull(services);

            services.TryAddSingleton<XHtmlMiddleware>();
            return services;
        }
    }
}