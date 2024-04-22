using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.ViewModels.Buckets;

public class BucketViewModel
{
    public string? Id { get; init; }
    public BucketLimitations? Limitations { get; set; }
    public string? StorageId { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public IEnumerable<BucketFile> Files { get; set; } = [];
}