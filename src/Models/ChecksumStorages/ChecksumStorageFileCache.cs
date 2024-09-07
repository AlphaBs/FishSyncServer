using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageFileCache
{
    private readonly IMemoryCache _cache;
    public string CacheNamespace { get; }
    
    public ChecksumStorageFileCache(IMemoryCache cache, string cacheNamespace)
    {
        _cache = cache;
        CacheNamespace = cacheNamespace;
    }
    
    public void SetFile(ChecksumStorageFile file)
    {
        _cache.Set(
            getCacheKey(file.Checksum), 
            file,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    public bool TryGetFile(string checksum, out ChecksumStorageFile? file)
    {
        return _cache.TryGetValue(getCacheKey(checksum), out file);
    }
    
    private string getCacheKey(string checksum) => $"{CacheNamespace}:{checksum}";
}