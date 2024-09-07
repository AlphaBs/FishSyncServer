using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageFileCacheFactory
{
    private readonly IMemoryCache _cache;

    public ChecksumStorageFileCacheFactory(IMemoryCache cache)
    {
        _cache = cache;
    }

    public ChecksumStorageFileCache Create(string cacheNamespace)
    {
        return new ChecksumStorageFileCache(_cache, cacheNamespace);
    }
}