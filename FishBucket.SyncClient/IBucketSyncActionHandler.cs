using FishBucket.SyncActions;
using FishSyncClient.Files;
using FishSyncClient.Progress;

namespace FishBucket.SyncClient;

public interface IBucketSyncActionHandler
{
    bool CanHandle(BucketSyncAction action);
    ValueTask Handle(
        SyncFile file,
        BucketSyncAction action,
        IProgress<ByteProgress>? progress,
        CancellationToken cancellationToken);
}