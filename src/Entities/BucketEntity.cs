using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Entities;

public class BucketEntity
{
    [Required]
    [MaxLength(16)]
    public required string Id { get; set; } = null!;

    [Required]
    [MaxLength(16)]
    public string Type { get; set; } = null!;
    
    public BucketLimitations Limitations { get; set; } = null!;
    public ICollection<UserEntity> Owners { get; } = [];
    public DateTimeOffset LastUpdated { get; set; }

    public ICollection<BucketEntity> Dependencies { get; set; } = [];
}