namespace AlphabetUpdateServer.Models.Buckets;

public class BucketLimitations
{
    public bool IsReadOnly { get; set; }
    public long MaxFileSize { get; set; }
    public long MaxNumberOfFiles { get; set; }
    public long MaxBucketSize { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
}