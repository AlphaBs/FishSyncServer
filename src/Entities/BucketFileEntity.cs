using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Entities;

[PrimaryKey(nameof(BucketId), nameof(Path))]
public class BucketFileEntity
{
    public string BucketId { get; set; } = null!;
    public string Path { get; set; } = null!;
    public long Size { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Checksum { get; set; } = null!;
}