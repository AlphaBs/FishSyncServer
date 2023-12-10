namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncActionFactory
{
    public BucketSyncAction FileSizeValidation(BucketSyncFile file) => new BucketSyncAction(file)
    {
        ActionType = BucketSyncActionTypes.Validation,
        Parameters = new Dictionary<string, string>
        {
            ["type"] = "size"
        }
    };

    public BucketSyncAction DuplicatedFilePath(BucketSyncFile file) => new BucketSyncAction(file)
    {
        ActionType = BucketSyncActionTypes.DuplicatedFilePath,
    };
}