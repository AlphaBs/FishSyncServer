namespace AlphabetUpdateServer.Models;

public interface IBucket
{
    string Id { get; }
    DateTime LastUpdated { get; set; }

    ValueTask<IEnumerable<BucketFile>> GetFiles();
    ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> files);
    ValueTask ClearFiles();
}