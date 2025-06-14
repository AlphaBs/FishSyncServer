namespace FishBucket;

public interface IBucket
{
    DateTimeOffset LastUpdated { get; }
    BucketLimitations Limitations { get; }
    
    ValueTask<IEnumerable<BucketFile>> GetFiles(CancellationToken cancellationToken);
    ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> syncFiles);
}