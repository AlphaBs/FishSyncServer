namespace AlphabetUpdateServer.Models.Buckets;

public record BucketFileMetadata(
    long Size,
    DateTimeOffset LastUpdated,
    string Checksum);