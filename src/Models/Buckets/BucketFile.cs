namespace AlphabetUpdateServer.Models.Buckets;

public record BucketFile(
    string BucketId,
    string Path,
    BucketFileMetadata Metadata);
