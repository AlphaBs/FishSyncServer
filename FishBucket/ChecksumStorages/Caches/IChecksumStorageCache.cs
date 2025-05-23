using FishBucket.ChecksumStorages.Storages;

namespace FishBucket.ChecksumStorages.Caches;

public interface IChecksumStorageCache
{
    Task<ChecksumStorageFile?> GetFile(string id, string checksum);
    Task SetFile(string id, ChecksumStorageFile file);
    Task SetFiles(string id, IEnumerable<ChecksumStorageFile> files);
    Task DeleteFiles(string id, IEnumerable<string> checksums);
}