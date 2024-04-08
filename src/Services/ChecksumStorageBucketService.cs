using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class ChecksumStorageBucketService
{
    private readonly RFilesChecksumStorageService _checksumStorageService;
    private readonly ApplicationDbContext _dbContext;

    public ChecksumStorageBucketService(
        RFilesChecksumStorageService checksumStorageService, 
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
            .FirstOrDefaultAsync();

        if (entity == null)
            return null;

        var checksumStorage = await _checksumStorageService.FindStorageById(entity.ChecksumStorageId);
        if (checksumStorage == null)
            throw new InvalidOperationException();

        var bucket = new ChecksumStorageBucket(
            limitations: entity.Limitations, 
            checksumStorage: checksumStorage);

        await bucket.UpdateFiles(
            entity.Files.Select(entityToFile), 
            entity.LastUpdated);
        return bucket;
    }

    public Task CreateBucket(string id, BucketLimitations limitations) =>
        CreateBucket(id, DateTimeOffset.UtcNow, limitations);

    public async Task CreateBucket(string id, DateTimeOffset lastUpdated, BucketLimitations limitations)
    {
        var bucket = new ChecksumStorageBucketEntity
        {
            Id = id,
            Limitations = limitations,
            LastUpdated = lastUpdated,
        };

        _dbContext.Buckets.Add(bucket);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBucket(string id, ChecksumStorageBucket bucket)
    {
        var entity = await _dbContext.Buckets
            .FirstOrDefaultAsync(entity => entity.Id == id);

        if (entity == null)
            throw new InvalidOperationException("Create a bucket first");

        var files = await bucket.GetFiles();
        entity.Files = files
            .Select(file => fileToEntity(id, file))
            .ToList();
        entity.LastUpdated = bucket.LastUpdated;
        entity.Limitations = bucket.Limitations;
    }

    private BucketFileEntity fileToEntity(string id, BucketFile file)
    {
        return new BucketFileEntity
        {
            BucketId = id,
            Path = file.Path,
            Location = file.Location,
            Metadata = file.Metadata
        };
    }

    private BucketFile entityToFile(BucketFileEntity entity)
    {
        return new BucketFile
        (
            Path: entity.Path,
            Location: entity.Location,
            Metadata: entity.Metadata
        );
    }
}