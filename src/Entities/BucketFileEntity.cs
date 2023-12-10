using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Entities;

[PrimaryKey(nameof(BucketId), nameof(Path))]
public class BucketFileEntity
{
    public string BucketId { get; set; } = null!;
    public string Path { get; set; } = null!;
    public long Size { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public string Checksum { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        var other = obj as BucketFileEntity;
        if (other == null)
            return false;
        
        return BucketId.Equals(other.BucketId)
            && Path.Equals(other.Path)
            && Size.Equals(other.Size)
            && LastUpdated.Equals(other.LastUpdated)
            && Checksum.Equals(other.Checksum);
    }

    public override int GetHashCode()
    {
        return BucketId.GetHashCode() ^ Path.GetHashCode();
    }
}