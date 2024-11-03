using AlphabetUpdateServer.Services.ChecksumStorageCaches;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CacheChecksumStorageFactory
{
    private readonly IChecksumStorageCache _cache;

    public CacheChecksumStorageFactory(IChecksumStorageCache cache)
    {
        _cache = cache;
    }

    public CacheChecksumStorage Create(string id, IChecksumStorage checksumStorage)
    {
        return new CacheChecksumStorage(id, checksumStorage, _cache);
    }
}