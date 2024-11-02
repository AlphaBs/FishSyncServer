using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class ChecksumStorageService
{
    private readonly IEnumerable<IChecksumStorageProvider> _providers;

    public ChecksumStorageService(IEnumerable<IChecksumStorageProvider> providers)
    {
        _providers = providers;
    }

    public async Task<IEnumerable<ChecksumStorageListItem>> GetAllStorages()
    {
        var items = Enumerable.Empty<ChecksumStorageListItem>();
        foreach (var provider in _providers)
        {
            var result = await provider.GetStorages();
            items = items.Concat(result);
        }
        return items;
    }

    public async Task<IChecksumStorage?> GetStorage(string id)
    {
        foreach (var provider in _providers)
        {
            var storage = await provider.GetStorage(id);
            if (storage != null)
                return storage;
        }

        return null;
    }
}