namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IChecksumStorage
{
    bool IsReadOnly { get; }
    IAsyncEnumerable<ChecksumStorageFile> GetAllFiles();
    Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums);
    Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums);
}