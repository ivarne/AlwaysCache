namespace AlwaysCache;

public interface IAlwaysCache<TCache>
{
    /// <summary>
    /// Get the current cache value (or throw exception if not loaded yet)
    /// </summary>
    public TCache Value { get; set; }
    public DateTime LastUpdate { get; }
}

public class DefaultAlwaysCache<TCache> : IAlwaysCache<TCache>
{
    private TCache? _internalValue;
    public TCache Value
    {
        get
        {
            return _internalValue ?? throw new InvalidOperationException($"Cache for {typeof(TCache).Name} not loaded");
        }
        set
        {
            LastUpdate = DateTime.Now;
            _internalValue = value;
        }

    }
    public DateTime LastUpdate { get; private set; }
}

public interface IAlwaysCacheInitializer<TCache>
{
    public Task<TCache> Initialize(CancellationToken cancellationToken);
}

public class DefaultAlwaysCacheInitializer<TCache>: IAlwaysCacheInitializer<TCache>
{
    private readonly Func<CancellationToken, Task<TCache>> _initialize;

    DefaultAlwaysCacheInitializer(Func<CancellationToken, Task<TCache>> initialize)
    {
        _initialize = initialize;
    }

    public async Task<TCache> Initialize(CancellationToken cancellationToken)
    {
        return await _initialize(cancellationToken);
    }
}
