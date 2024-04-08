using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.Entities;

public class BucketFileEntity
{
    public string BucketId { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? Location { get; set; }
    public FileMetadata Metadata { get; set; } = null!;
}