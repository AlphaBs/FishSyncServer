namespace AlphabetUpdateServer.Models.Buckets;

public class FileSizeValidationAction : BucketSyncAction
{
    public FileSizeValidationAction(BucketSyncFile file) : base(file)
    {
        ActionType = BucketSyncActionTypes.Validation;
        Parameters["type"] = "size";
    }
}