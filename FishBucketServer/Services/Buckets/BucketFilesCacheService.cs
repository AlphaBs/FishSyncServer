using Microsoft.Extensions.Caching.Memory;

namespace AlphabetUpdateServer.Services.Buckets;

public class BucketFilesCacheService
{
    private readonly BucketService _bucketService;
    private readonly IMemoryCache _memoryCache;

    private static readonly MemoryCacheEntryOptions Options = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(60),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
    };

    public BucketFilesCacheService(IMemoryCache memoryCache, BucketService bucketService)
    {
        _memoryCache = memoryCache;
        _bucketService = bucketService;
    }

    public async Task<BucketFiles> GetOrCreate(string id, CancellationToken cancellationToken)
    {
        var result = await _memoryCache.GetOrCreateAsync(
            key(id),
            _ => _bucketService.GetBucketFiles(id, cancellationToken), 
            Options);

        return result ?? await _bucketService.GetBucketFiles(id, cancellationToken);
    }

    public void Remove(string id)
    {
        _memoryCache.Remove(key(id));
    }
    
    private string key(string id) => $"BucketFiles:{id}";
}