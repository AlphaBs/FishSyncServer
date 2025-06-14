namespace FishBucket.ChecksumStorages.Storages;

public class ChecksumStorageSyncResult
{
    public IReadOnlyCollection<ChecksumStorageFile> SuccessFiles { get; }
    public IReadOnlyCollection<ChecksumStorageSyncAction> RequiredActions { get; }

    public ChecksumStorageSyncResult(
        IReadOnlyCollection<ChecksumStorageFile> successFiles, 
        IReadOnlyCollection<ChecksumStorageSyncAction> requiredActions)
    {
        SuccessFiles = successFiles;
        RequiredActions = requiredActions;
    }
}

public record ChecksumStorageSyncAction
(
    string Checksum,
    SyncAction Action
);