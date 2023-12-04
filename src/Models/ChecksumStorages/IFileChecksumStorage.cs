using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IFileChecksumStorage
{
    bool IsReadOnly { get; }
    IAsyncEnumerable<FileLocation> GetAllFiles();
    IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums);
    BucketSyncAction CreateSyncAction(string checksum);
}