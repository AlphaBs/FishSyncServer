using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageBucketFileEntity
{
    public string BucketId { get; set; } = default!;
    public string Path { get; set; } = default!;
    public FileMetadata Metadata { get; set; } = default!; 
}