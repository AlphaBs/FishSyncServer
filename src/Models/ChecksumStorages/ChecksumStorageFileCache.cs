using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageFileCache
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false,
    };
    
    private readonly IDistributedCache _cache;
    public string CacheNamespace { get; }
    
    public ChecksumStorageFileCache(IDistributedCache cache, string cacheNamespace)
    {
        _cache = cache;
        CacheNamespace = cacheNamespace;
    }
    
    public async Task SetFile(ChecksumStorageFile file)
    {
        await _cache.SetAsync(
            getCacheKey(file.Checksum), 
            JsonSerializer.SerializeToUtf8Bytes(file, SerializerOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
    }

    public async Task<ChecksumStorageFile?> GetFile(string checksum)
    {
        var data = await _cache.GetAsync(getCacheKey(checksum));
        if (data == null)
            return null;
        return JsonSerializer.Deserialize<ChecksumStorageFile>(data);
    }
    
    private string getCacheKey(string checksum) => $"{CacheNamespace}:{checksum}";
}