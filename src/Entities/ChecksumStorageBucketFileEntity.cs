using System.ComponentModel.DataAnnotations;
using FishBucket;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageBucketFileEntity
{
    [Required]
    [MaxLength(64)]
    public string BucketId { get; set; } = default!;
    
    [Required]
    [MaxLength(256)]
    public string Path { get; set; } = default!;
    
    [Required]
    public FileMetadata Metadata { get; set; } = default!; 
}