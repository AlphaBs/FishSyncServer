using FishBucket;

namespace AlphabetUpdateServer.DTOs;

public class BucketResponse
{
    public required string Id { get; set; }
    public required DateTimeOffset LastUpdated { get; set; }
    public required BucketLimitations Limitations { get; set; }
    public required IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();
    public required IAsyncEnumerable<string> Owners { get; set; } = AsyncEnumerable.Empty<string>();
}