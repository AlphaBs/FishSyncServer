using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageFileCacheEntity
{
    public string StorageId { get; set; } = null!;
    public string Checksum { get; set; } = null!;
    public bool Exists { get; set; }
    public DateTimeOffset CachedAt { get; set; }

    public string? Location { get; set; }
    public long Size { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

    public bool TryGetMetadata(out FileMetadata metadata)
    {
        if (Exists)
        {
            metadata = new FileMetadata(Size, LastUpdated, Checksum);
            return true;
        }
        else
        {
            metadata = null!;
            return false;
        }
    }
}