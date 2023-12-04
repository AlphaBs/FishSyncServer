namespace AlphabetUpdateServer.Models.Buckets;

public interface IBucket
{
    string Id { get; }
    DateTime LastUpdated { get; set; }

    IAsyncEnumerable<BucketFile> GetFiles();
    ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> files);
    ValueTask ClearFiles();
}