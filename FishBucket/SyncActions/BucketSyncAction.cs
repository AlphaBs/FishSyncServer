using System.Text.Json.Serialization;

namespace FishBucket.SyncActions;

public record BucketSyncAction
(
    [property:JsonPropertyName("path")]
    string Path,
    [property:JsonPropertyName("action")]
    SyncAction Action
);