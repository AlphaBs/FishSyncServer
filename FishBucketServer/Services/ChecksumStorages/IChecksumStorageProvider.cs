using FishBucket.ChecksumStorages.Storages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public interface IChecksumStorageProvider
{
    IAsyncEnumerable<ChecksumStorageListItem> GetStorages();
    Task<IChecksumStorage?> GetStorage(string id);
}