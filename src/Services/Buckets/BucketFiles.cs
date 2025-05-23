using FishBucket;

namespace AlphabetUpdateServer.Services.Buckets;

public class BucketFiles
{
    public required string Id { get; set; }
    public required DateTimeOffset LastUpdated { get; set; }
    public required IEnumerable<BucketFile> Files { get; set; } = [];
    public required IEnumerable<string> Dependencies { get; set; } = [];
}