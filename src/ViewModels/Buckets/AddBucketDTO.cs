using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.ViewModels;

public class AddBucketViewModel
{
    public string? Id { get; set; }
    public BucketLimitations? Limitations { get; set; }
}