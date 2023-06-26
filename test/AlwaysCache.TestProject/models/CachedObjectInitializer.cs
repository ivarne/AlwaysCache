namespace AlwaysCache.TestProject.Models;

public class CachedObjectInitializer : IAlwaysCacheInitializer<CachedObject>
{
    private readonly ILogger<CachedObjectInitializer> _logger;

    public CachedObjectInitializer(ILogger<CachedObjectInitializer> logger)
    {
        _logger = logger;
    }
    public async Task<CachedObject> Initialize(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting waiting for cached value of type");
        await Task.Delay(10000, cancellationToken);
        _logger.LogInformation("Finished waiting for cached value");
        return new CachedObject
        {
            Opprettet = DateTime.Now,
            Text = "Dette er en cachet verdi",
        };
    }
}