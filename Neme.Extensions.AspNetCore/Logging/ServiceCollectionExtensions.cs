using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Neme.Extensions.AspNetCore.Logging;
using Neme.Extensions.Contracts;
using System.Reflection;

namespace Neme.Extensions.MicrosoftExtensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResponseLogger(
        this IServiceCollection services,
        LogLevel logLevel = LogLevel.Information)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpContextAccessor();

        services.TryAddScoped(typeof(ResponseLogger<>), serviceProvider =>
        {
            // Get the generic type argument T from the requested service type
            var requestedType = (Type)serviceProvider.GetType()
                .GetProperty("ServiceType")!
                .GetValue(serviceProvider)
                .NotNull();

            if (!requestedType.IsGenericType)
                throw new InvalidOperationException("ResponseLogger<T> must be requested with a generic type argument.");

            var genericArgument = requestedType.GetGenericArguments()[0];

            var method = typeof(ServiceCollectionExtensions)
                .GetMethod(nameof(CreateResponseLogger), BindingFlags.NonPublic | BindingFlags.Static)
                .NotNull();

            var genericMethod = method.MakeGenericMethod(genericArgument);
            return genericMethod.Invoke(null, [serviceProvider, logLevel]).NotNull();
        });

        return services;
    }

    private static ResponseLogger<T> CreateResponseLogger<T>(IServiceProvider serviceProvider, LogLevel logLevel)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext.NotNull();

        var logger = serviceProvider.GetRequiredService<ILogger<ResponseLogger<T>>>();

        return new ResponseLogger<T>(httpContext.Response, logLevel, logger);
    }
}
