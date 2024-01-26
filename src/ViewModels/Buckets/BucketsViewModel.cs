using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.ViewModels.Buckets;

public class BucketsViewModel
{
    public ChecksumBaseBucket[] Buckets { get; init; } = null!;
}