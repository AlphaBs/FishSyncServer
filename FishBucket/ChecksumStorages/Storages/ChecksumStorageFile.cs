namespace FishBucket.ChecksumStorages.Storages;

public record ChecksumStorageFile
(
    string Checksum,
    string Location,
    FileMetadata Metadata
);