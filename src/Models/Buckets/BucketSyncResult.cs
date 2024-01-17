namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncResult
{
    public static BucketSyncResult Success(IEnumerable<BucketFile> files, DateTimeOffset updatedAt) =>
        new BucketSyncResult(
            true, 
            Enumerable.Empty<BucketSyncAction>(), 
            files,
            updatedAt);

    public static BucketSyncResult ActionRequired(BucketSyncAction action) =>
        ActionRequired([action]);

    public static BucketSyncResult ActionRequired(IEnumerable<BucketSyncAction> actions) => 
        new BucketSyncResult(
            false, 
            actions,
            Enumerable.Empty<BucketFile>(), 
            DateTimeOffset.MinValue);

    public BucketSyncResult(
        bool isSuccess, 
        IEnumerable<BucketSyncAction> actions, 
        IEnumerable<BucketFile> files,
        DateTimeOffset updatedAt) =>
        (IsSuccess, RequiredActions, Files, UpdatedAt) = 
        (isSuccess, actions, files, updatedAt);

    public bool IsSuccess { get; }
    public IEnumerable<BucketSyncAction> RequiredActions { get; }
    public IEnumerable<BucketFile> Files { get; }
    public DateTimeOffset UpdatedAt { get; }
}