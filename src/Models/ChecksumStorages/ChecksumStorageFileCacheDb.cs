using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageFileCacheDb : IChecksumStorageFileCacheRepository
{
    private readonly string _storageId;
    private readonly ApplicationDbContext _dbContext;

    public ChecksumStorageFileCacheDb(string storageId, ApplicationDbContext dbContext) =>
        (_storageId, _dbContext) = (storageId, dbContext);

    public async Task<IEnumerable<ChecksumStorageFileCache>> GetAllCaches()
    {
        var caches = await _dbContext.ChecksumStorageFileCaches
            .Where(entity => entity.StorageId == _storageId)
            .ToListAsync();
        return caches.Select(convertEntityToCache);
    }

    public async Task<IEnumerable<ChecksumStorageFileCache>> Query(IEnumerable<string> checksums)
    {
        var entities = await _dbContext.ChecksumStorageFileCaches
            .Where(entity => entity.StorageId == _storageId && checksums.Contains(entity.Checksum))
            .ToListAsync();
        return entities.Select(convertEntityToCache);
    }

    public void AddCache(ChecksumStorageFileCache cache)
    {
        var entity = convertCacheToEntity(cache);
        _dbContext.Add(entity);
    }

    public void UpdateCache(ChecksumStorageFileCache cache)
    {
        var entity = convertCacheToEntity(cache);
        _dbContext.Update(entity);
    }

    public void RemoveCaches(IEnumerable<string> checksums)
    {
        var checksumList = checksums.ToList();
        var deleteEntities = _dbContext.ChecksumStorageFileCaches
            .Where(entity => entity.StorageId == _storageId && checksumList.Contains(entity.Checksum));
        _dbContext.ChecksumStorageFileCaches.RemoveRange(deleteEntities);
    }

    public async Task RemoveAllCaches()
    {
        await _dbContext.ChecksumStorageFileCaches
            .Where(entity => entity.StorageId == _storageId)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    private ChecksumStorageFileCache convertEntityToCache(ChecksumStorageFileCacheEntity entity)
    {
        if (entity.Exists)
        {
            if (!string.IsNullOrEmpty(entity.Location) && entity.TryGetMetadata(out var metadata))
            {
                return ChecksumStorageFileCache.CreateExistentCache(entity.Checksum, entity.Location, metadata);
            }
            else
            {
                return ChecksumStorageFileCache.CreateNonExistentCache(entity.Checksum);
            }
        }
        else
        {
            return ChecksumStorageFileCache.CreateNonExistentCache(entity.Checksum);
        }
    }

    private ChecksumStorageFileCacheEntity convertCacheToEntity(ChecksumStorageFileCache cache)
    {
        if (cache.TryGetLocation(out var location) && cache.TryGetMetadata(out var metadata))
        {
            return new ChecksumStorageFileCacheEntity
            {
                StorageId = _storageId,
                Checksum = cache.Checksum,
                Exists = true,
                CachedAt = cache.CachedAt,

                Location = location,
                Size = metadata.Size,
                LastUpdated = metadata.LastUpdated
            };
        }
        else
        {
            return new ChecksumStorageFileCacheEntity
            {
                StorageId = _storageId,
                Checksum = cache.Checksum,
                Exists = false,
                CachedAt = cache.CachedAt,

                Location = null,
                Size = 0,
                LastUpdated = DateTimeOffset.MinValue
            };
        }
    }
}