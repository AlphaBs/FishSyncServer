namespace AlphabetUpdateServer.Models.Buckets;

[Serializable]
public class BucketLimitationException : Exception
{
    public static readonly string ReadonlyBucket = "readonly_bucket";
    public static readonly string ExpiredBucket = "expired_bucket";
    public static readonly string ExceedMaxBucketSize = "exceed_max_bucket_size";
    public static readonly string ExceedMaxNumberOfFiles = "exceed_max_number_of_files";

    public BucketLimitationException(string reason) => Reason = reason;
    public BucketLimitationException(string reason, string message) : base(message) => Reason = reason;
    public BucketLimitationException(string reason, string message, Exception inner) : base(message, inner) => Reason = reason;

    public string Reason { get; }
}