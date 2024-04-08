namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IChecksumStorageFileCacheRepository
{
    Task<IEnumerable<ChecksumStorageFileCache>> GetAllCaches();
    Task<IEnumerable<ChecksumStorageFileCache>> Query(IEnumerable<string> checksums);
    void AddCache(ChecksumStorageFileCache cache);
    void UpdateCache(ChecksumStorageFileCache cache);
    Task RemoveCaches(IEnumerable<string> checksums);
    Task RemoveAllCaches();
    Task SaveChanges();
}