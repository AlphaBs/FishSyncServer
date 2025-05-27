using FishBucket;

namespace AlphabetUpdateServer.Services.Buckets;

public interface IBucketService
{
    string Type { get; }
    Task<IBucket?> Find(string id);
    Task<BucketSyncResult> Sync(string id, IEnumerable<BucketSyncFile> syncFiles);
}