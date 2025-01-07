using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.DTOs;

public class BucketFilesDTO
{
    public string? Id { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public IReadOnlyCollection<BucketFile> Files { get; set; } = [];
    public IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();
}