namespace AlphabetUpdateServer.Services.Buckets;

public record BucketListItem
(
    string Id,
    string Type,
    IEnumerable<string> Owners,
    DateTimeOffset LastUpdated
);