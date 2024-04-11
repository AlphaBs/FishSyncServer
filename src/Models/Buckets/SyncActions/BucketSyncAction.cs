namespace AlphabetUpdateServer.Models.Buckets.SyncActions;

public record BucketSyncAction
(
    string Path,
    SyncAction Action
);