namespace AlphabetUpdateServer.Services.Buckets;

public record BucketListItem
(
    string Id,
    IEnumerable<string> Owners,
    DateTimeOffset LastUpdated
);