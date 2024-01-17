using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.DTOs;

public class BucketDTO
{
    public string? Id { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }
    public BucketLimitations? Limitations { get; set; }
    
}