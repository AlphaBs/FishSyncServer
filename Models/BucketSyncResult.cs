namespace AlphabetUpdateServer.Models;

public class BucketSyncResult
{
    public static BucketSyncResult Success() =>
        new BucketSyncResult(true, Enumerable.Empty<BucketSyncAction>());

    public static BucketSyncResult ActionRequired(IEnumerable<BucketSyncAction> actions) => 
        new BucketSyncResult(false, actions);

    public BucketSyncResult(bool isSuccess, IEnumerable<BucketSyncAction> actions) =>
        (IsSuccess, RequiredActions) = (isSuccess, actions);

    public bool IsSuccess { get; }
    public IEnumerable<BucketSyncAction> RequiredActions { get; }
}