namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageFileCache
{
    public static ChecksumStorageFileCache CreateExistentCache(string checksum, string location, FileMetadata metadata) =>
        CreateExistentCache(checksum, location, metadata, DateTimeOffset.UtcNow);

    public static ChecksumStorageFileCache CreateExistentCache(
        string checksum, string location, FileMetadata metadata, DateTimeOffset cachedAt) =>
        new ChecksumStorageFileCache(checksum, true, cachedAt, location, metadata);

    public static ChecksumStorageFileCache CreateNonExistentCache(string checksum) =>
        CreateNonExistentCache(checksum, DateTimeOffset.UtcNow);

    public static ChecksumStorageFileCache CreateNonExistentCache(string checksum, DateTimeOffset cachedAt) =>
        new ChecksumStorageFileCache(checksum, false, cachedAt, null, null);

    private ChecksumStorageFileCache(
        string checksum,
        bool exists,
        DateTimeOffset cachedAt,
        string? location,
        FileMetadata? metadata) =>
        (Checksum, Exists, CachedAt, _location, _metadata) = 
        (checksum, exists, cachedAt, location, metadata);

    public string Checksum { get; }
    public bool Exists { get; } = false;
    public DateTimeOffset CachedAt { get; }

    private readonly string? _location;
    public bool TryGetLocation(out string location)
    {
        if (Exists)
        {
            location = _location!;
            return true;
        }
        else
        {
            location = null!;
            return false;
        }
    }

    private readonly FileMetadata? _metadata;
    public bool TryGetMetadata(out FileMetadata metadata)
    {
        if (Exists)
        {
            metadata = _metadata!;
            return true;
        }
        else
        {
            metadata = null!;
            return false;
        }
    }
}