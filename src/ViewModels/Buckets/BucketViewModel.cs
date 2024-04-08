using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.ViewModels.Buckets;

public class BucketViewModel
{
    public string BucketId { get; init; } = null!;
    public ChecksumStorageBucket Bucket { get; init; } = null!;
    public IEnumerable<BucketFile> Files { get; init; } = null!;
}