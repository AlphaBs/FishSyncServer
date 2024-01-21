using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.ChecksumStorages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class ChecksumStorageService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEnumerable<IFileChecksumStorageConverter> _converters;

    public ChecksumStorageService(
        ApplicationDbContext dbContext,
        IEnumerable<IFileChecksumStorageConverter> converters)
    {
        _dbContext = dbContext;
        _converters = converters;
    }

    public async Task<IEnumerable<FileChecksumStorageEntity>> GetAllEntities(string bucketId)
    {
        return await _dbContext.ChecksumStorages
            .Where(storage => storage.BucketId == bucketId)
            .OrderBy(storage => storage.Priority)
            .ToListAsync();
    }

    public async Task<IEnumerable<IFileChecksumStorage>> GetAllStorages(string bucketId)
    {
        var storageEntities = await GetAllEntities(bucketId);
        var storages = storageEntities.Select(toStorageFromEntity);
        return storages;
    }

    public async Task<IFileChecksumStorage> CreateStorageForBucket(string bucketId)
    {
        var storages = await GetAllStorages(bucketId);

        var composite = new CompositeFileChecksumStorage();
        foreach (var storage in storages)
        {
            composite.AddStorage(storage);
        }

        var cached = new CachedFileChecksumStorage(composite, _dbContext);
        return cached;
    }

    public async Task<FileChecksumStorageEntity?> GetEntityById(string bucketId, string storageId)
    {
        return await _dbContext.ChecksumStorages
            .Where(s => s.BucketId == bucketId && s.StorageId == storageId)
            .FirstOrDefaultAsync();
    }

    public async Task AddStorage(FileChecksumStorageEntity entity)
    {
        _dbContext.ChecksumStorages.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> RemoveStorage(string bucketId, string storageId)
    {
        var deleted = await _dbContext.ChecksumStorages
            .Where(storage => storage.BucketId == bucketId  && storage.StorageId == storageId)
            .ExecuteDeleteAsync();
        return deleted > 0;
    }

    private IFileChecksumStorage toStorageFromEntity(FileChecksumStorageEntity entity)
    {
        return _converters
            .First(converter => converter.CanConvert(entity))
            .ConvertFromEntityToStorage(entity);
    }
}