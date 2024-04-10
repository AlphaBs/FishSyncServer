using AlphabetUpdateServer.Services;

namespace AlphabetUpdateServer.ViewModels.Buckets;

public class BucketsViewModel
{
    public IReadOnlyCollection<BucketListItem> Buckets { get; init; } = null!;
}