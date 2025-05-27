using FishBucket.ChecksumStorages.Caches;
using FishBucket.ChecksumStorages.Storages;
using Microsoft.Extensions.Caching.Memory;

namespace FishBucket.Tests.ChecksumStorages;

public class ChecksumFileStorageCacheTests
{
    private static string TestStorageId = "test";
    
    [Fact]
    public async Task get_files_caches_all_checksums()
    {
        var cache = createCache();
        var storage = createStorage(cache);
        var files = await storage.GetAllFiles();
        Assert.Equal(["existing_checksum1", "existing_checksum2", "existing_checksum3"], files.Select(file => file.Checksum));
        await AssertCacheExists(cache, "existing_checksum1");
        await AssertCacheExists(cache, "existing_checksum2");
        await AssertCacheExists(cache, "existing_checksum3");
    }

    [Fact]
    public async Task query_caches_found_files()
    {
        var cache = createCache();
        var storage = createStorage(cache);
        var query = new[]
        { 
            "existing_checksum1", 
            "not_existing_checksum1", 
            "existing_checksum2", 
            "not_existing_checksum2"
        };
        
        var result = await storage.Query(query);
        Assert.Equal(["existing_checksum1", "existing_checksum2"], result.FoundFiles.Select(file => file.Checksum));
        await AssertCacheExists(cache, "existing_checksum1");
        await AssertCacheExists(cache, "existing_checksum2");
        Assert.Equal(["not_existing_checksum1", "not_existing_checksum2"], result.NotFoundChecksums);
    }

    [Fact]
    public async Task query_cached_files()
    {
        var inMemoryStorage = new InMemoryChecksumStorage();
        inMemoryStorage.Add(createFile("existing_checksum1", "existing_location1"));
        inMemoryStorage.Add(createFile("existing_checksum2", "existing_location2"));
        inMemoryStorage.Add(createFile("existing_checksum3", "existing_location3"));
        var storage = new CacheChecksumStorage(TestStorageId, inMemoryStorage, createCache());
        
        // cache files
        await storage.Query(["existing_checksum1", "existing_checksum2"]);
        
        // remove original files
        inMemoryStorage.Clear();
        
        // return files from cache
        var result = await storage.Query(["existing_checksum1", "existing_checksum2", "not_existing_checksum"]);
        Assert.Equal(["existing_checksum1", "existing_checksum2"], result.FoundFiles.Select(file => file.Checksum));
        Assert.Equal(["not_existing_checksum"], result.NotFoundChecksums);
    }

    [Fact]
    public async Task cache_sync_result()
    {
        var cache = createCache();
        var storage = createStorage(cache);

        var result = await storage.Sync(["existing_checksum1", "existing_checksum2", "existing_checksum3", "not_existing_checksum"]);
        Assert.Equal(["existing_checksum1", "existing_checksum2", "existing_checksum3"], result.SuccessFiles.Select(file => file.Checksum));
        Assert.Equal(["not_existing_checksum"], result.RequiredActions.Select(action => action.Checksum));
        await AssertCacheExists(cache, "existing_checksum1");
        await AssertCacheExists(cache, "existing_checksum2");
        await AssertCacheExists(cache, "existing_checksum3");
    }

    [Fact]
    public async Task sync_cached_files()
    {
        var cache = createCache();
        var inMemoryStorage = new InMemoryChecksumStorage();
        inMemoryStorage.Add(createFile("existing_checksum1", "existing_location1"));
        inMemoryStorage.Add(createFile("existing_checksum2", "existing_location2"));
        inMemoryStorage.Add(createFile("existing_checksum3", "existing_location3"));
        var storage = new CacheChecksumStorage(TestStorageId, inMemoryStorage, cache);
        
        // cache files
        await storage.Sync(["existing_checksum1", "existing_checksum2"]);
        
        // remove original files
        inMemoryStorage.Clear();
        
        // return files from cache
        var result = await storage.Sync(["existing_checksum1", "existing_checksum2", "not_existing_checksum"]);
        Assert.Equal(["existing_checksum1", "existing_checksum2"], result.SuccessFiles.Select(file => file.Checksum));
        Assert.Equal(["not_existing_checksum"], result.RequiredActions.Select(action => action.Checksum));
    }
    
    private static InMemoryChecksumStorageCache createCache()
    {
        return new InMemoryChecksumStorageCache(new MemoryCache(new MemoryCacheOptions()));
    }
    
    private static CacheChecksumStorage createStorage(IChecksumStorageCache cache)
    {
        var inMemoryStorage = new InMemoryChecksumStorage();
        inMemoryStorage.Add(createFile("existing_checksum1", "existing_location1"));
        inMemoryStorage.Add(createFile("existing_checksum2", "existing_location2"));
        inMemoryStorage.Add(createFile("existing_checksum3", "existing_location3"));
        return new CacheChecksumStorage(TestStorageId, inMemoryStorage, cache);
    }
    
    private static ChecksumStorageFile createFile(string checksum, string location)
    {
        return new ChecksumStorageFile(checksum, location, new FileMetadata(1, DateTimeOffset.MinValue, checksum));
    }
    
    private static async Task AssertCacheExists(IChecksumStorageCache cache, string checksum)
    {
        var cachedFile = await cache.GetFile(TestStorageId, checksum);
        Assert.NotNull(cachedFile);
        Assert.Equal(checksum, cachedFile?.Checksum);
    }
}