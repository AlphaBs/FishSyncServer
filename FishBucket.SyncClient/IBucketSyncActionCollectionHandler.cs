using FishBucket.SyncActions;
using FishSyncClient.Files;

namespace FishBucket.SyncClient;

public interface IBucketSyncActionCollectionHandler
{
    Task Handle(ISyncFileCollection files, IEnumerable<BucketSyncAction> actions, CancellationToken cancellationToken);
}