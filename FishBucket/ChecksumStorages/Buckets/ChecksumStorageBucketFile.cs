namespace FishBucket.ChecksumStorages.Buckets;

public record ChecksumStorageBucketFile
(
    string Path,
    FileMetadata Metadata
);