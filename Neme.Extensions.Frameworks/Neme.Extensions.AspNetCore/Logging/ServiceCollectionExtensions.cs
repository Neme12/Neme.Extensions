using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Neme.Extensions.AspNetCore.Logging;

namespace Neme.Extensions.MicrosoftExtensions.Logging;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddResponseLogger()
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddHttpContextAccessor();
            services.TryAddScoped(typeof(ResponseLogger<>), typeof(ResponseLogger<>));

            return services;
        }

        public IServiceCollection AddResponseLogger(
            LogLevel logLevel)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure<ResponseLoggerOptions>(options =>
            {
                options.LogLevel = logLevel;
            });

            services.AddResponseLogger();

            return services;
        }
    }
}
