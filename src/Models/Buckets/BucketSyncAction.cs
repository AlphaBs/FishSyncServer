namespace AlphabetUpdateServer.Models.Buckets;

public record BucketSyncAction
(
    string Path,
    SyncAction Action
);