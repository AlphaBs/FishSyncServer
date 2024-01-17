namespace AlphabetUpdateServer.Models.Buckets;

public record BucketFileLocation(
    string Path, 
    string? Location,
    BucketFileMetadata Metadata);