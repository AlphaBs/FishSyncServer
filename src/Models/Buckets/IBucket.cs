namespace AlphabetUpdateServer.Models.Buckets;

public interface IBucket
{
    DateTimeOffset LastUpdated { get; }
    BucketLimitations Limitations { get; }

    ValueTask<IEnumerable<BucketFile>> GetFiles();
    ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> syncFiles);
    ValueTask UpdateFiles(IEnumerable<BucketFile> files, DateTimeOffset updatedAt);
}