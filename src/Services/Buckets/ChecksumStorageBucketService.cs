using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.ChecksumStorages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace AlphabetUpdateServer.Services.Buckets;

public class ChecksumStorageBucketService
{
    private readonly ChecksumStorageService _checksumStorageService;
    private readonly ConfigService _configService;
    private readonly ApplicationDbContext _dbContext;

    public ChecksumStorageBucketService(
        ChecksumStorageService checksumStorageService,
        ConfigService configService,
        ApplicationDbContext dbContext)
    {
        _checksumStorageService = checksumStorageService;
        _configService = configService;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<BucketListItem>> GetAllBuckets()
    {
        return await _dbContext.Buckets
            .Select(bucket => new BucketListItem(
                bucket.Id,
                bucket.Owners.Select(owner => owner.Username), 
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
        if (await _configService.GetMaintenanceMode())
            throw new ServiceMaintenanceException();
        
        var entity = await _dbContext.Buckets
            .Include(e => e.Files)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (entity == null)
            throw new KeyNotFoundException(id);
        
        var files = await bucket.GetFiles();
        var fileEntities = files
            .Select(file => fileToEntity(id, file))
            .ToList();

        entity.Files.Clear();
        entity.Files.AddRange(fileEntities);
        entity.LastUpdated = bucket.LastUpdated;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLimitations(string id, BucketLimitations limitations)
    {
        if (await _configService.GetMaintenanceMode())
            throw new ServiceMaintenanceException();

        var entity = await _dbContext.Buckets
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (entity == null)
            throw new KeyNotFoundException(id);

        entity.Limitations = limitations;
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task UpdateStorageId(string id, string storageId)
    {
        if (await _configService.GetMaintenanceMode())
            throw new ServiceMaintenanceException();

        var rows = await _dbContext.Buckets
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.ChecksumStorageId, storageId));

        if (rows == 0)
            throw new KeyNotFoundException(id);
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