using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class UserEntity
{
    [Required] 
    [MaxLength(16)] 
    public required string Username { get; set; } = null!;

    [Required]
    [MaxLength(128)]
    public string HashedPassword { get; set; } = null!;
    
    public IList<string> Roles { get; set; } = [];
    
    public ICollection<ChecksumStorageBucketEntity> Buckets { get; set; } = [];
    
    [EmailAddress]
    [MaxLength(128)]
    public string? Email { get; set; }
    
    [MaxLength(128)]
    public string? Memo { get; set; }

    [ConcurrencyCheck]
    [MaxLength(40)]
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
}