using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageBucketEntity : BucketEntity
{
    public const string ChecksumStorageType = "checksum-storage";
    
    public ChecksumStorageBucketEntity()
    {
        Type = ChecksumStorageType;
    }
    
    public ICollection<ChecksumStorageBucketFileEntity> Files { get; } = [];
    
    [Required]
    [MaxLength(16)]
    public string ChecksumStorageId { get; set; } = null!;
}