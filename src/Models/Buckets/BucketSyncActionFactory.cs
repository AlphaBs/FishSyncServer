namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncActionFactory
{
    public static BucketSyncAction ExpiredBucket() =>
        BucketValidation("expired_bucket");

    public static BucketSyncAction ReadOnlyBucket() => 
        BucketValidation("readonly_bucket");
    
    public static BucketSyncAction ExceedMaxBucketSize() =>
        BucketValidation("exceed_max_bucket_size");

    public static BucketSyncAction BucketValidation(string type) => new BucketSyncAction
    {
        ActionType = BucketSyncActionTypes.BucketValidation,
        Parameters = new Dictionary<string, string>
        {
            ["type"] = type
        }
    };

    public static BucketSyncAction InvalidFileSize(BucketSyncFile file) =>
        FileValidation(file, "invalid_file_size");

    public static BucketSyncAction WrongFileSize(BucketSyncFile file) =>
        FileValidation(file, "wrong_file_size");

    public static BucketSyncAction ExceedMaxFileSize(BucketSyncFile file) =>
        FileValidation(file, "exceed_max_file_size");

    public static BucketSyncAction FileValidation(BucketSyncFile file, string type) => new BucketSyncAction
    {
        File = file,
        ActionType = BucketSyncActionTypes.FileValidation,
        Parameters = new Dictionary<string, string>
        {
            ["type"] = type
        }
    };

    public static BucketSyncAction DuplicatedFilePath(BucketSyncFile file) => new BucketSyncAction
    {
        File = file,
        ActionType = BucketSyncActionTypes.DuplicatedFilePath,
    };
}