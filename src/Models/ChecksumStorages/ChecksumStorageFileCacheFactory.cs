using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageFileCacheFactory
{
    private readonly IDistributedCache _cache;

    public ChecksumStorageFileCacheFactory(IDistributedCache cache)
    {
        _cache = cache;
    }

    public ChecksumStorageFileCache Create(string cacheNamespace)
    {
        return new ChecksumStorageFileCache(_cache, cacheNamespace);
    }
}