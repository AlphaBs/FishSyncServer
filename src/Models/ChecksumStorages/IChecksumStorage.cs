using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IChecksumStorage
{
    bool IsReadOnly { get; }
    IAsyncEnumerable<ChecksumStorageFile> GetAllFiles();
    IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums);
    SyncAction CreateSyncAction(string checksum);
}