using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlwaysCache;

public static class ServiceCollectionExtentions
{
    // public static IServiceCollection AddAlwaysCache<TCache>(this IServiceCollection services, Func<CancellationToken,Task<TCache>> initializer)
    // {
    //     services.AddSingleton<IAlwaysCache<TCache>, DefaultAlwaysCache<TCache>>();
    //     services.AddHostedService<AlwaysCacheBackgroundService<TCache, TCacheInitializer>>();
    //     return services;
    // }
    public static IServiceCollection AddAlwaysCache<TCache, TCacheInitializer>(this IServiceCollection services) where TCacheInitializer: class, IAlwaysCacheInitializer<TCache>
    {
        services.AddSingleton<IAlwaysCache<TCache>, DefaultAlwaysCache<TCache>>();
        services.AddTransient<IAlwaysCacheInitializer<TCache>, TCacheInitializer>();
        services.AddHostedService<AlwaysCacheBackgroundService<TCache, IAlwaysCacheInitializer<TCache>>>();
        return services;
    }

    public static async Task WaitForAlwaysCache(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var hostedServices = scope.ServiceProvider.GetServices<IHostedService>().OfType<AlwaysCacheBackgroundServiceBase>();
        var tasks = hostedServices.Select(bgCacheService=>bgCacheService.UpdateCache(CancellationToken.None));
        await Task.WhenAll(tasks);
    }
}