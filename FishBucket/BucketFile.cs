namespace FishBucket;

public record BucketFile(
    string Path, 
    string? Location,
    FileMetadata Metadata);