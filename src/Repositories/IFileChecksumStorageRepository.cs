using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Repositories;

public interface IFileChecksumStorageRepository
{
    ValueTask<IEnumerable<FileChecksumStorageEntity>> GetStorages(string bucketId);
    ValueTask<FileChecksumStorageEntity?> FindStorage(string bucketId, string id);
    ValueTask Add(FileChecksumStorageEntity storage);
    ValueTask Remove(string bucketId, string id);
}