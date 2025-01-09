using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.DTOs;

public class BucketDTO
{
    public required string Id { get; set; }
    public required DateTimeOffset LastUpdated { get; set; }
    public required BucketLimitations Limitations { get; set; }
    public required IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();
    public required IAsyncEnumerable<string> Owners { get; set; } = AsyncEnumerable.Empty<string>();
}