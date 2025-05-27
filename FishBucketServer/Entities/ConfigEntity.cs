using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class ConfigEntity
{
    [Required]
    [MaxLength(16)]
    public required string Id { get; set; }
    
    [Required]
    [MaxLength(64)]
    public required string Value { get; set; }
}