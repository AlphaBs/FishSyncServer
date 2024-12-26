using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.DTOs;

public class BucketDTO
{
    public string? Id { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }
    public BucketLimitations? Limitations { get; set; }
    public IReadOnlyCollection<BucketFile> Files { get; set; } = [];
    public IReadOnlyCollection<string> Dependencies { get; set; } = [];
}