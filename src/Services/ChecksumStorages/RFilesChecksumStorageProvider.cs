using AlphabetUpdateServer.Models.ChecksumStorages;

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

    public async Task<IEnumerable<ChecksumStorageListItem>> GetStorages()
    {
        return await _storageService.GetAllItems();
    }

    public async Task<IChecksumStorage?> GetStorage(string id)
    {
        var entity = await _storageService.FindEntityById(id);
        if (entity == null)
        {
            return null;
        }
        return new RFilesChecksumStorage(entity.Host, entity.ClientSecret, entity.IsReadonly, _httpClientFactory.CreateClient());
    }
}