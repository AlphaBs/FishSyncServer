using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class ObjectChecksumStorageEntity : ChecksumStorageEntity
{
    public static readonly string ObjectType = "object";

    public ObjectChecksumStorageEntity()
    {
        Type = ObjectType;
    }

    [Required]
    [MaxLength(128)]
    public string AccessKey { get; set; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string SecretKey { get; set; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string BucketName { get; set; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string Prefix { get; set; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string ServiceEndpoint { get; set; } = null!;
    
    [Required]
    [MaxLength(128)]
    public string PublicEndpoint { get; set; } = null!;
}