using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageBucketEntity
{
    [Required]
    [MaxLength(16)]
    public required string Id { get; set; } = null!;
    
    public BucketLimitations Limitations { get; set; } = null!;
    
    public ICollection<UserEntity> Owners { get; } = [];
    
    public ICollection<ChecksumStorageBucketFileEntity> Files { get; } = [];
    
    public DateTimeOffset LastUpdated { get; set; }
    
    [Required]
    [MaxLength(16)]
    public string ChecksumStorageId { get; set; } = null!;
}