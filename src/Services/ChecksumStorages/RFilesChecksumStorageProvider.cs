using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class RFilesChecksumStorageProvider : IChecksumStorageProvider
{
    private readonly RFilesChecksumStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ChecksumStorageFileCacheFactory _cache;

    public RFilesChecksumStorageProvider(
        IHttpClientFactory httpClientFactory,
        RFilesChecksumStorageService storageService,
        ChecksumStorageFileCacheFactory cache)
    {
        _httpClientFactory = httpClientFactory;
        _storageService = storageService;
        _cache = cache;
    }

    public async Task<IEnumerable<ChecksumStorageListItem>> GetStorages()
    {
        return await _storageService.GetAllItems();
    }

    public async Task<IChecksumStorage?> GetStorage(string id)
    {
        var entity = await _storageService.FindEntityById(id);
        if (entity == null)
            return null;
        
        var storage = new RFilesChecksumStorage(
            entity.Host, 
            entity.ClientSecret, 
            entity.IsReadonly, 
            _httpClientFactory.CreateClient());
        var cache = _cache.Create($"RFilesChecksumStorage.{id}");
        return new CacheChecksumStorage(storage, cache);
    }
}