using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class ChecksumStorageService
{
    private readonly IEnumerable<IChecksumStorageProvider> _providers;
    private readonly CacheChecksumStorageFactory _cacheFactory;

    public ChecksumStorageService(
        IEnumerable<IChecksumStorageProvider> providers,
        CacheChecksumStorageFactory cacheFactory)
    {
        _providers = providers;
        _cacheFactory = cacheFactory;
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
            {
                var cachedStorage = _cacheFactory.Create(id, storage);
                return cachedStorage;
            }
        }

        return null;
    }
}