using FishBucket.ChecksumStorages.Storages;
using Microsoft.Extensions.Caching.Memory;

namespace FishBucket.ChecksumStorages.Caches;

public class InMemoryChecksumStorageCache : IChecksumStorageCache
{
    private readonly IMemoryCache _cache;

    public InMemoryChecksumStorageCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    private string getKey(string id, string checksum) => $"{id}:{checksum}";
    
    public Task<ChecksumStorageFile?> GetFile(string id, string checksum)
    {
        var result = _cache.Get<ChecksumStorageFile>(getKey(id, checksum));
        return Task.FromResult(result);
    }

    public Task SetFile(string id, ChecksumStorageFile file)
    {
        _cache.Set(getKey(id, file.Checksum), file);
        return Task.CompletedTask;
    }

    public Task SetFiles(string id, IEnumerable<ChecksumStorageFile> files)
    {
        foreach (var file in files)
        {
            SetFile(id, file);
        }

        return Task.CompletedTask;
    }

    public Task DeleteFiles(string id, IEnumerable<string> checksums)
    {
        foreach (var checksum in checksums)
        {
            _cache.Remove(getKey(id, checksum));
        }

        return Task.CompletedTask;
    }
}