namespace FishBucket.ChecksumStorages.Storages;

public interface IChecksumStorage
{
    bool IsReadOnly { get; }
    Task<IEnumerable<ChecksumStorageFile>> GetAllFiles();
    Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums);
    Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums);
}