namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncActionFactory
{
    public static BucketSyncAction InvalidFileSize(BucketSyncFile file) =>
        FileValidation(file, "invalid_file_size");

    public static BucketSyncAction WrongFileSize(BucketSyncFile file) =>
        FileValidation(file, "wrong_file_size");

    public static BucketSyncAction ExceedMaxFileSize(BucketSyncFile file) =>
        FileValidation(file, "exceed_max_file_size");

    public static BucketSyncAction FileValidation(BucketSyncFile file, string type) => new BucketSyncAction
    (
        Path: file.Path ?? throw new ArgumentException("empty file path"),
        Action: new SyncAction
        (
            Type: BucketSyncActionTypes.FileValidation,
            Parameters: new Dictionary<string, string>()
            {
                ["type"] = type
            }
        )
    );

    public static BucketSyncAction DuplicatedFilePath(BucketSyncFile file) => new BucketSyncAction
    (
        Path: file.Path ?? throw new ArgumentException("empty file path"),
        Action: new SyncAction
        (
            Type: BucketSyncActionTypes.DuplicatedFilePath,
            Parameters: null
        )
    );

    
}