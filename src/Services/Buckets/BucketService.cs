using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.Buckets;

public class BucketService
{
    private readonly BucketServiceFactory _bucketServiceFactory;
    private readonly ConfigService _configService;
    private readonly ApplicationDbContext _dbContext;

    public BucketService(ConfigService configService, ApplicationDbContext dbContext, BucketServiceFactory bucketServiceFactory)
    {
        _configService = configService;
        _dbContext = dbContext;
        _bucketServiceFactory = bucketServiceFactory;
    }

    public async Task<IEnumerable<BucketListItem>> GetAllBuckets()
    {
        return await _dbContext.Buckets
            .Select(bucket => new BucketListItem(
                bucket.Id,
                bucket.Type,
                bucket.Owners.Select(owner => owner.Username), 
                bucket.LastUpdated))
            .ToListAsync();
    }

    public async Task<BucketListItem?> FindBucketItem(string id)
    {
        return await _dbContext.Buckets
            .Select(bucket => new BucketListItem(
                bucket.Id,
                bucket.Type,
                bucket.Owners.Select(owner => owner.Username),
                bucket.LastUpdated))
            .FirstOrDefaultAsync();
    }

    private async Task<BucketEntity?> findEntityById(string id) =>
        await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
    
    public async Task<IBucket?> Find(string id)
    {
        var entity = await findEntityById(id);
        if (entity == null)
            return null;

        var service = _bucketServiceFactory.GetRequiredService(entity.Type);
        return await service.Find(id);
    }

    public async Task<BucketSyncResult> SyncAndLog(string bucketId, string userId, IEnumerable<BucketSyncFile> syncFiles)
    {
        if (await _configService.GetMaintenanceMode())
            throw new ServiceMaintenanceException();
        
        var entity = await findEntityById(bucketId);
        if (entity == null)
            throw new KeyNotFoundException(bucketId);

        var syncCount = await GetMonthlySuccessfulSyncCount(bucketId);
        if (syncCount >= entity.Limitations.MonthlyMaxSyncCount)
            throw new BucketLimitationException(BucketLimitationException.ExceedMonthlySyncCount);

        var service = _bucketServiceFactory.GetRequiredService(entity.Type);
        var result = await service.Sync(bucketId, syncFiles);
        if (result.IsSuccess)
        {
            addSuccessEvent(bucketId, userId);
        }
        else
        {
            addActionRequiredEvent(bucketId, userId);
        }
        
        await _dbContext.SaveChangesAsync();
        return result;
    }

    public async Task<int> GetMonthlySuccessfulSyncCount(string bucketId)
    {
        var now = DateTimeOffset.UtcNow;
        var startTimestamp = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
        return await _dbContext.BucketSyncEvents
            .Where(e => e.BucketId == bucketId)
            .Where(e => e.EventType == BucketSyncEventType.Success)
            .Where(e => e.Timestamp >= startTimestamp)
            .CountAsync();
    }
    
    private void addSuccessEvent(string bucketId, string userId)
    {
        var entity = new BucketSyncEventEntity
        {
            BucketId = bucketId,
            UserId = userId,
            Timestamp = DateTimeOffset.Now,
            EventType = BucketSyncEventType.Success
        };
        _dbContext.BucketSyncEvents.Add(entity);
    }

    private void addActionRequiredEvent(string bucketId, string userId)
    {
        var entity = new BucketSyncEventEntity()
        {
            BucketId = bucketId,
            UserId = userId,
            Timestamp = DateTimeOffset.Now,
            EventType = BucketSyncEventType.ActionRequired
        };
        _dbContext.BucketSyncEvents.Add(entity);
    }
    
    public async Task<IReadOnlyCollection<BucketSyncEventEntity>> GetAllSyncEvents()
    {
        return await _dbContext.BucketSyncEvents
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();        
    }

    public async Task<IReadOnlyCollection<BucketSyncEventEntity>> GetSyncEvents(string bucketId)
    {
        return await _dbContext.BucketSyncEvents
            .Where(e => e.BucketId == bucketId)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();
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

    public async Task<IReadOnlyCollection<string>> GetDependencies(string id)
    {
        var entity = await _dbContext.Buckets
            .IgnoreAutoIncludes()
            .Include(e => e.Dependencies)
            .Where(e => e.Id == id)
            .Select(e => e.Dependencies
                .Select(b => b.Id))
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new KeyNotFoundException(id);

        return entity.ToList();
    }

    public async Task AddDependency(string id, string dependencyId)
    {
        var bucket = new BucketEntity { Id = id };
        _dbContext.Buckets.Attach(bucket);

        var dep = new BucketEntity { Id = dependencyId };
        _dbContext.Buckets.Attach(dep);
        
        bucket.Dependencies.Add(dep);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveDependency(string id, string dependencyId)
    {
        var entity = await _dbContext.Buckets
            .IgnoreAutoIncludes()
            .Include(e => e.Dependencies)
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new KeyNotFoundException(id);
        
        entity.Dependencies.Remove(entity.Dependencies.First(e => e.Id == dependencyId));
        await _dbContext.SaveChangesAsync();
    }
}