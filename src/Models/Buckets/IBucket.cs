namespace AlphabetUpdateServer.Models.Buckets;

public interface IBucket
{
    string Id { get; }
    DateTimeOffset LastUpdated { get; }
    BucketLimitations Limitations { get; set; }

    IAsyncEnumerable<BucketFileLocation> GetFiles(IEnumerable<BucketFile> files);
    ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> syncFiles);
}