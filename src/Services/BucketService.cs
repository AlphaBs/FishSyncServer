using AlphabetUpdateServer.Models.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class BucketService
{
    private readonly ApplicationDbContext _dbContext;

    public BucketService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ChecksumBaseBucket>> GetAllBuckets()
    {
        var buckets = await _dbContext.Buckets.ToListAsync();
        return buckets;
    }

    public async Task<ChecksumBaseBucket?> GetBucketById(string id)
    {
        var bucket = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
        return bucket;
    }

    public Task AddNewBucket(string id, BucketLimitations limitations) =>
        AddNewBucket(id, DateTimeOffset.UtcNow, limitations);

    public async Task AddNewBucket(string id, DateTimeOffset lastUpdated, BucketLimitations limitations)
    {
        var bucket = new ChecksumBaseBucket(id, lastUpdated, limitations);
        _dbContext.Buckets.Add(bucket);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBucket(ChecksumBaseBucket bucket)
    {
        await _dbContext.SaveChangesAsync();
    }
}