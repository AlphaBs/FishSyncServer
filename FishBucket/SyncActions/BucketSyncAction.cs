namespace FishBucket.SyncActions;

public record BucketSyncAction
(
    string Path,
    SyncAction Action
);