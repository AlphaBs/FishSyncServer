namespace AlphabetUpdateServer.Services.Buckets;

public record BucketListItem
(
    string Id,
    string Type,
    DateTimeOffset LastUpdated
);