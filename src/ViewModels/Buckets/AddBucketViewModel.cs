using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.ViewModels.Buckets;

public class AddBucketViewModel
{
    public string? Id { get; set; }
    public BucketLimitations? Limitations { get; set; }
}