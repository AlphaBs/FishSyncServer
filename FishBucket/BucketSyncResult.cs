using System.Text.Json.Serialization;
using FishBucket.SyncActions;

namespace FishBucket;

public class BucketSyncResult
{
    public static BucketSyncResult Success(DateTimeOffset updatedAt) =>
        new BucketSyncResult(
            true, 
            [], 
            updatedAt);

    public static BucketSyncResult ActionRequired(BucketSyncAction action) =>
        ActionRequired([action]);

    public static BucketSyncResult ActionRequired(IReadOnlyList<BucketSyncAction> actions) => 
        new BucketSyncResult(
            false, 
            actions,
            DateTimeOffset.MinValue);

    public BucketSyncResult(
        bool isSuccess, 
        IReadOnlyList<BucketSyncAction> actions,
        DateTimeOffset updatedAt) =>
        (IsSuccess, RequiredActions, UpdatedAt) = 
        (isSuccess, actions, updatedAt);

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; }
    
    [JsonPropertyName("requiredActions")]
    public IReadOnlyList<BucketSyncAction> RequiredActions { get; }
    
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; }
}