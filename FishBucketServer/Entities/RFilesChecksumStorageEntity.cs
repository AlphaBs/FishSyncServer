using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class RFilesChecksumStorageEntity : ChecksumStorageEntity
{
    public const string RFilesType = "rfiles";

    public RFilesChecksumStorageEntity()
    {
        Type = RFilesType;
    }

    [Required]
    [MaxLength(128)]
    public string Host { get; set; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string? ClientSecret { get; set; }
}