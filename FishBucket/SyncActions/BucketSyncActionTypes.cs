namespace FishBucket.SyncActions;

public static class BucketSyncActionTypes
{
    public const string Http = "http";
    public const string FileValidation = "file_validation";
    public const string DuplicatedFilePath = "duplicated_filepath";
    public const string UnknownError = "error";
}