namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncResult
{
    public static BucketSyncResult Success() =>
        new BucketSyncResult(true, Enumerable.Empty<BucketSyncAction>(), null);

    public static BucketSyncResult Error(string message) =>
        new BucketSyncResult(false, Enumerable.Empty<BucketSyncAction>(), message);

    public static BucketSyncResult ActionRequired(IEnumerable<BucketSyncAction> actions) => 
        new BucketSyncResult(false, actions, null);

    public BucketSyncResult(
        bool isSuccess, 
        IEnumerable<BucketSyncAction> actions, 
        string? message) =>
        (IsSuccess, RequiredActions, Message) = 
        (isSuccess, actions, message);

    public bool IsSuccess { get; }
    public string? Message { get; }
    public IEnumerable<BucketSyncAction> RequiredActions { get; }
}