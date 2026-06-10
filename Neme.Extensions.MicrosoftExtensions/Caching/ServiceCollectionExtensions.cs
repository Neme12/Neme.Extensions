using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NodaTime;

namespace Neme.Extensions.MicrosoftExtensions.Caching;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileCache(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<FileCache>();
        services.TryAddSingleton<IFileCache>(sp => sp.GetRequiredService<FileCache>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, FileCacheCleanupService>());

        return services;
    }

    public static IServiceCollection AddFileCache(
        this IServiceCollection services,
        Action<FileCacheOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.AddFileCache();
        services.Configure(setupAction);

        return services;
    }
}
