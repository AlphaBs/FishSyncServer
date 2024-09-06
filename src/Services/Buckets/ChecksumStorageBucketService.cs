using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class ChecksumStorageBucketService
{
    private readonly ChecksumStorageService _checksumStorageService;
    private readonly ApplicationDbContext _dbContext;

    public ChecksumStorageBucketService(
        ChecksumStorageService checksumStorageService, 
        ApplicationDbContext dbContext)
    {
        _checksumStorageService = checksumStorageService;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<BucketListItem>> GetAllBuckets()
    {
        return await _dbContext.Buckets
            .Select(bucket => new BucketListItem(
                bucket.Id,
                bucket.Owners.Select(owner => owner.Id), 
                bucket.LastUpdated))
            .ToListAsync();
    }

    public async Task<ChecksumStorageBucket?> FindBucketById(string id)
    {
        var entity = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .Include(bucket => bucket.Files)
            .FirstOrDefaultAsync();

        if (entity == null)
            return null;

        var checksumStorage = await _checksumStorageService.GetStorage(entity.ChecksumStorageId);
        if (checksumStorage == null)
            throw new InvalidOperationException();

        var bucket = new ChecksumStorageBucket(
            limitations: entity.Limitations, 
            checksumStorage: checksumStorage);

        bucket.UpdateFiles(
            entity.Files.Select(entityToFile), 
            entity.LastUpdated);
        return bucket;
    }

    public Task CreateBucket(string id, BucketLimitations limitations, string storageId) =>
        CreateBucket(id, DateTimeOffset.UtcNow, limitations, storageId);

    public async Task CreateBucket(
        string id, 
        DateTimeOffset lastUpdated, 
        BucketLimitations limitations, 
        string storageId)
    {
        var bucket = new ChecksumStorageBucketEntity
        {
            Id = id,
            Limitations = limitations,
            LastUpdated = lastUpdated,
            ChecksumStorageId = storageId,
        };

        _dbContext.Buckets.Add(bucket);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateFiles(string id, ChecksumStorageBucket bucket)
    {
        var bucketEntity = await _dbContext.Buckets
            .FirstOrDefaultAsync(entity => entity.Id == id);
        if (bucketEntity == null)
            throw new InvalidOperationException("Create bucket first");

        var files = await bucket.GetFiles();
        bucketEntity.Files.Clear();
        bucketEntity.Files.AddRange(files.Select(file => fileToEntity(id, file)));
        bucketEntity.LastUpdated = bucket.LastUpdated;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLimitationsAndStorageId(string id, BucketLimitations limitations, string storageId)
    {
        var entity = await _dbContext.Buckets
            .FirstOrDefaultAsync(entity => entity.Id == id);
        if (entity == null)
            throw new InvalidOperationException("Create bucket first");

        entity.Limitations = limitations;
        entity.ChecksumStorageId = storageId;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> GetStorageId(string id)
    {
        return await _dbContext.Buckets
            .Where(entity => entity.Id == id)
            .Select(entity => entity.ChecksumStorageId)
            .FirstAsync();
    }

    private ChecksumStorageBucketFile entityToFile(ChecksumStorageBucketFileEntity entity)
    {
        return new ChecksumStorageBucketFile(entity.Path, entity.Metadata);
    }

    private ChecksumStorageBucketFileEntity fileToEntity(string bucketId, BucketFile file)
    {
        return new ChecksumStorageBucketFileEntity
        {
            BucketId = bucketId,
            Path = file.Path,
            Metadata = file.Metadata,
        };
    }
}