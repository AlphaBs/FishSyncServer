namespace FishBucket;

public record SyncAction
(
    string Type,
    IReadOnlyDictionary<string, string>? Parameters
);