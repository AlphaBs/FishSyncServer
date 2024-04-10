namespace AlphabetUpdateServer.Services;

public record BucketListItem
(
    string Id,
    IEnumerable<string> Owners,
    DateTimeOffset LastUpdated
);