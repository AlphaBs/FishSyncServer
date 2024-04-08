namespace AlphabetUpdateServer.Models.Buckets;

public record BucketFile(
    string Path, 
    string? Location,
    FileMetadata Metadata);