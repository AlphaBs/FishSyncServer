namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageSyncResult
{
    public IReadOnlyCollection<ChecksumStorageFile> SuccessFiles { get; }
    public IReadOnlyCollection<SyncAction> RequiredActions { get; }

    public ChecksumStorageSyncResult(
        IReadOnlyCollection<ChecksumStorageFile> successFiles, 
        IReadOnlyCollection<SyncAction> requiredActions)
    {
        SuccessFiles = successFiles;
        RequiredActions = requiredActions;
    }
}