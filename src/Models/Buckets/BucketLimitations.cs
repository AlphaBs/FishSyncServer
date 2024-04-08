namespace AlphabetUpdateServer.Models.Buckets;

public class BucketLimitations
{
    public readonly static BucketLimitations NoLimits = new BucketLimitations
    {
        IsReadOnly = false,
        MaxFileSize = int.MaxValue,
        MaxNumberOfFiles = int.MaxValue,
        MaxBucketSize = int.MaxValue,
        ExpiredAt = DateTimeOffset.MaxValue
    };

    public bool IsReadOnly { get; set; }
    public long MaxFileSize { get; set; }
    public long MaxNumberOfFiles { get; set; }
    public long MaxBucketSize { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
}