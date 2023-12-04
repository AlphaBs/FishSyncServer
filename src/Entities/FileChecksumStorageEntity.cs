using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Entities;

[PrimaryKey(nameof(BucketId), nameof(StorageId))]
public class FileChecksumStorageEntity
{
    public string? BucketId { get; set; }
    public virtual BucketEntity? Bucket { get; set; }
    public string StorageId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsReadyOnly { get; set; }
    public int Priority { get; set; }
    public string? Location { get; set; }
}