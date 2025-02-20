using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageEntity
{
    [Required]
    [MaxLength(16)]
    public string Id { get; set; } = null!;
    
    [Required]
    [MaxLength(16)]
    public string Type { get; set; } = null!;
    
    public bool IsReadonly { get; set; }
}