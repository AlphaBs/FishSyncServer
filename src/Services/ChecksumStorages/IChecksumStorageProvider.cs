using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public interface IChecksumStorageProvider
{
    Task<IEnumerable<ChecksumStorageListItem>> GetStorages();
    Task<IChecksumStorage?> GetStorage(string id);
}