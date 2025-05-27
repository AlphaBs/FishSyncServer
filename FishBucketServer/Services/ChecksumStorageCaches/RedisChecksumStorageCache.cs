using System.Text.Json;
using FishBucket.ChecksumStorages.Caches;
using FishBucket.ChecksumStorages.Storages;
using StackExchange.Redis;

namespace AlphabetUpdateServer.Services.ChecksumStorageCaches;

public class RedisChecksumStorageCache : IChecksumStorageCache
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false,
    };

    private readonly ConnectionMultiplexer _redis;
    private readonly TimeSpan _expiry;
    
    public RedisChecksumStorageCache(
        ConnectionMultiplexer redis,
        IConfiguration config)
    {
        _redis = redis;
        
        var expiryString = config["RedisChecksumStorageCache:Expiry"] ??
                           throw new ArgumentException("Expiry was empty");
        _expiry = TimeSpan.Parse(expiryString);
    }
    
    public async Task<ChecksumStorageFile?> GetFile(string id, string checksum)
    {
        var db = _redis.GetDatabase();
        var v = await db.StringGetAsync(getCacheKey(id, checksum));
        var data = (byte[]?)v;
        if (data == null)
            return null;
        return JsonSerializer.Deserialize<ChecksumStorageFile>(data);
    }
    
    public Task SetFile(string id, ChecksumStorageFile file)
    {
        _redis.GetDatabase().StringSet(
            getCacheKey(id, file.Checksum),
            JsonSerializer.SerializeToUtf8Bytes(file, SerializerOptions),
            _expiry,
            When.Always,
            CommandFlags.FireAndForget);
        return Task.CompletedTask;
    }

    public async Task SetFiles(string id, IEnumerable<ChecksumStorageFile> files)
    {
        var tasks = files.Select(file => SetFile(id, file));
        await Task.WhenAll(tasks);
    }
    
    public IEnumerable<string> GetAllCacheKeys(string id)
    {
        var endpoints = _redis.GetEndPoints();
        var prefix = $"{getCacheNamespace(id)}:*";
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            var keys = server.Keys(pattern: prefix);
            foreach (string? key in keys)
            {
                if (key is not null)
                    yield return key.Substring(prefix.Length);
            }
        }
    }

    public async Task DeleteFiles(string id, IEnumerable<string> checksums)
    {
        var keys = checksums
            .Select(checksum => (RedisKey)getCacheKey(id, checksum))
            .ToArray();
        await _redis.GetDatabase().KeyDeleteAsync(keys, CommandFlags.FireAndForget);
    }
    
    private string getCacheNamespace(string id) => $"FishSyncServer:ChecksumStorageCache:{id}";
    private string getCacheKey(string id, string checksum) => $"{getCacheNamespace(id)}:{checksum}";
}