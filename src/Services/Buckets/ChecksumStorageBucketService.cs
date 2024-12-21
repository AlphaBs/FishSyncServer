using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.ChecksumStorages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace AlphabetUpdateServer.Services.Buckets;

public class ChecksumStorageBucketService : IBucketService
{
    private readonly ConfigService _configService;
    private readonly ChecksumStorageService _checksumStorageService;
    private readonly ApplicationDbContext _dbContext;

    public ChecksumStorageBucketService(
        ConfigService configService,
        ChecksumStorageService checksumStorageService,
        ApplicationDbContext dbContext)
    {
        _configService = configService;
        _checksumStorageService = checksumStorageService;
        _dbContext = dbContext;
    }

    public const string ChecksumStorageType = ChecksumStorageBucketEntity.ChecksumStorageType;
    public string Type => ChecksumStorageType;
    
    private async Task<ChecksumStorageBucketEntity?> findEntityById(string id) =>
        await _dbContext.ChecksumStorageBuckets
            .Where(bucket => bucket.Id == id)
            .Include(bucket => bucket.Files)
            .FirstOrDefaultAsync();

    private async Task<ChecksumStorageBucket> createBucketFromEntity(ChecksumStorageBucketEntity entity)
    {
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
    
    public async Task<IBucket?> Find(string id)
    {
        var entity = await findEntityById(id);
        if (entity == null)
            return null;
        return await createBucketFromEntity(entity);
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
    
    public async Task<BucketSyncResult> Sync(string bucketId, IEnumerable<BucketSyncFile> syncFiles)
    {
        var entity = await findEntityById(bucketId);
        if (entity == null)
            throw new KeyNotFoundException(bucketId);
        
        var bucket = await createBucketFromEntity(entity);
        var result = await bucket.Sync(syncFiles);
        if (result.IsSuccess)
        {
            var bucketFiles = await bucket.GetFiles();
            var bucketFileEntities = bucketFiles
                .Select(file => fileToEntity(bucketId, file))
                .ToList();
            
            entity.Files.Clear();
            entity.Files.AddRange(bucketFileEntities);
            entity.LastUpdated = bucket.LastUpdated;
        }

        return result;
    }
    
    public async Task UpdateStorageId(string id, string storageId)
    {
        if (await _configService.GetMaintenanceMode())
            throw new ServiceMaintenanceException();

        var rows = await _dbContext.ChecksumStorageBuckets
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.ChecksumStorageId, storageId));

        if (rows == 0)
            throw new KeyNotFoundException(id);

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<string> GetStorageId(string id)
    {
        return await _dbContext.ChecksumStorageBuckets
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