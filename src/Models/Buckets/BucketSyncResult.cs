namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncResult
{
    public static BucketSyncResult Success(DateTimeOffset updatedAt) =>
        new BucketSyncResult(
            true, 
            Enumerable.Empty<BucketSyncAction>(), 
            updatedAt);

    public static BucketSyncResult ActionRequired(BucketSyncAction action) =>
        ActionRequired([action]);

    public static BucketSyncResult ActionRequired(IEnumerable<BucketSyncAction> actions) => 
        new BucketSyncResult(
            false, 
            actions,
            DateTimeOffset.MinValue);

    public BucketSyncResult(
        bool isSuccess, 
        IEnumerable<BucketSyncAction> actions,
        DateTimeOffset updatedAt) =>
        (IsSuccess, RequiredActions, UpdatedAt) = 
        (isSuccess, actions, updatedAt);

    public bool IsSuccess { get; }
    public IEnumerable<BucketSyncAction> RequiredActions { get; }
    public DateTimeOffset UpdatedAt { get; }
}