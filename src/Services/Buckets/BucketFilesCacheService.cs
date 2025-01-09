using AlphabetUpdateServer.Models.Buckets;
using Microsoft.Extensions.Caching.Memory;

namespace AlphabetUpdateServer.Services.Buckets;

public class BucketFilesCacheService
{
    private readonly IMemoryCache _memoryCache;

    private static readonly MemoryCacheEntryOptions Options = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(60),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
    };

    public BucketFilesCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<BucketFiles> GetOrCreate(string id, Func<Task<BucketFiles>> factory)
    {
        var result = await _memoryCache.GetOrCreateAsync(
            key(id), 
            _ => factory(), 
            Options);

        return result ?? await factory();
    }

    public void Remove(string id)
    {
        _memoryCache.Remove(key(id));
    }
    
    private string key(string id) => $"BucketFiles:{id}";
}