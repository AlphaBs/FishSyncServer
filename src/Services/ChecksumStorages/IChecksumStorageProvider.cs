using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public interface IChecksumStorageProvider
{
    IAsyncEnumerable<ChecksumStorageListItem> GetStorages();
    Task<IChecksumStorage?> GetStorage(string id);
}