using FishBucket.ChecksumStorages.Storages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class RFilesChecksumStorageProvider : IChecksumStorageProvider
{
    private readonly RFilesChecksumStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;

    public RFilesChecksumStorageProvider(
        IHttpClientFactory httpClientFactory,
        RFilesChecksumStorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _storageService = storageService;
    }

    public IAsyncEnumerable<ChecksumStorageListItem> GetStorages()
    {
        return _storageService.GetAllItems();
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
        return storage;
    }
}