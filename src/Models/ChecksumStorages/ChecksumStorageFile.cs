namespace AlphabetUpdateServer.Models.ChecksumStorages;

public record ChecksumStorageFile
(
    string Checksum,
    string Location,
    FileMetadata Metadata
);