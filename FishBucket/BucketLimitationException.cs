namespace FishBucket;

[Serializable]
public class BucketLimitationException : Exception
{
    public const string ReadonlyBucket = "readonly_bucket";
    public const string ExpiredBucket = "expired_bucket";
    public const string ExceedMaxBucketSize = "exceed_max_bucket_size";
    public const string ExceedMaxNumberOfFiles = "exceed_max_number_of_files";
    public const string ExceedMonthlySyncCount = "exceed_monthly_sync_count";

    public BucketLimitationException(string reason) => Reason = reason;
    public BucketLimitationException(string reason, string message) : base(message) => Reason = reason;
    public BucketLimitationException(string reason, string message, Exception inner) : base(message, inner) => Reason = reason;

    public string Reason { get; }
}