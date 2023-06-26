using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlwaysCache;

public abstract class AlwaysCacheBackgroundServiceBase: BackgroundService
{
    public abstract Task UpdateCache(CancellationToken none);
}

public class AlwaysCacheBackgroundService<TCache, TCacheInitializer> : AlwaysCacheBackgroundServiceBase where TCacheInitializer : IAlwaysCacheInitializer<TCache>
{
    private readonly IAlwaysCache<TCache> _cache;
    private readonly ILogger<AlwaysCacheBackgroundService<TCache, TCacheInitializer>> _logger;
    private readonly TCacheInitializer _initializer;

    public AlwaysCacheBackgroundService(IAlwaysCache<TCache> cache, ILogger<AlwaysCacheBackgroundService<TCache,TCacheInitializer>> logger, TCacheInitializer initializer)
    {
        _cache = cache;
        _logger = logger;
        _initializer = initializer;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(_cache is DefaultAlwaysCache<TCache> cache)
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if((DateTime.Now - cache.LastUpdate) > TimeSpan.FromSeconds(30))
                {
                    _logger.LogInformation($"Updating cache for {typeof(TCache).Name}");
                    var cachedValue = await _initializer.Initialize(stoppingToken);
                    _logger.LogInformation($"Updating cache for {typeof(TCache).Name}");
                    _cache.Value = cachedValue;
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch(Exception e) when (e is not OperationCanceledException)
            {
                _logger.LogCritical(e, "Loading cache data for {type} failed", typeof(TCache).Name);
            }
        }
    }
    /// <summary>
    /// This might be run before <see cref="ExcecuteAsync"/>
    /// if the user wants to ensure that cache is availible before booting the app
    /// </summary>
    public override async Task UpdateCache(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Updating cache for {typeof(TCache).Name}");
        var cachedValue = await _initializer.Initialize(stoppingToken);
        _cache.Value = cachedValue;
        _logger.LogInformation($"Finished updating cache for {typeof(TCache).Name}");
    }
}