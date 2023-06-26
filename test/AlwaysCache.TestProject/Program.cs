using AlwaysCache;
using AlwaysCache.TestProject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAlwaysCache<CachedObject, CachedObjectInitializer>();

var app = builder.Build();

await app.Services.WaitForAlwaysCache();

app.MapGet("/", (IAlwaysCache<CachedObject> cache) =>cache.Value);

app.Run();
