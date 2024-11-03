using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorageCaches;

public interface IChecksumStorageCache
{
    Task<ChecksumStorageFile?> GetFile(string id, string checksum);
    Task SetFile(string id, ChecksumStorageFile file);
    Task SetFiles(string id, IEnumerable<ChecksumStorageFile> files);
    Task DeleteFiles(string id, IEnumerable<string> checksums);
}