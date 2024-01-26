using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.ViewModels.Buckets;

public class BucketViewModel
{
    public ChecksumBaseBucket Bucket { get; init; } = null!;
    public IEnumerable<BucketFileLocation> Files { get; init; } = null!;
}