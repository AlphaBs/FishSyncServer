using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Models.ChecksumStorages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class BucketService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileChecksumStorageFactory _checksumStorageManager;

    public BucketService(
        ApplicationDbContext context,
        IFileChecksumStorageFactory checksumStorageManager)
    {
        _dbContext = context;
        _checksumStorageManager = checksumStorageManager;
    }

    public Task<IEnumerable<Bucket>> GetAllBuckets()
    {
        return _dbContext.Buckets.ToAsyncEnumerable();
    }

    public async Task<Bucket> GetBucketById(string id)
    {
        return await _dbContext.Buckets.SingleOrDefaultAsync(bucket => bucket.Id == id);
    }

    public async IAsyncEnumerable<BucketFileLocation> GetFiles(Bucket bucket)
    {
        var bucketFiles = await _dbContext.BucketFiles
            .Where(file => file.BucketId == bucket.Id)
            .ToListAsync();

        var checksumStorage = await _checksumStorageManager.CreateStorageForBucket(bucket.Id);
        var fileLocations = bucket.GetFiles(checksumStorage, bucketFiles);
        await foreach (var fileLocation in fileLocations)
        {
            yield return fileLocation;
        }
    }

    public async Task ClearFiles(string bucketId)
    {
        await _dbContext.BucketFiles.Where(file => file.BucketId == bucketId).ExecuteDeleteAsync();
    }

    public async ValueTask<BucketSyncResult> Sync(Bucket bucket, IEnumerable<BucketSyncFile> syncFiles)
    {
        var checksumStorage = await _checksumStorageManager.CreateStorageForBucket(bucket.Id);
        var syncResult = await bucket.Sync(checksumStorage, syncFiles);
        if (syncResult.IsSuccess)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.BucketFiles.Where(file => file.BucketId == bucket.Id).ExecuteDeleteAsync();
            await _dbContext.BucketFiles.AddRangeAsync(syncResult.Files);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        return syncResult;
    }
}