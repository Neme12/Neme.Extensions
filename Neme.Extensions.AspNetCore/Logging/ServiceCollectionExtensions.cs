using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Neme.Extensions.AspNetCore.Logging;

namespace Neme.Extensions.MicrosoftExtensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResponseLogger(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpContextAccessor();
        services.TryAddScoped(typeof(ResponseLogger<>), typeof(ResponseLogger<>));

        return services;
    }

    public static IServiceCollection AddResponseLogger(
        this IServiceCollection services,
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
