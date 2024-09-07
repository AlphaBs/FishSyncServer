using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class ObjectChecksumStorageProvider : IChecksumStorageProvider
{
    private readonly ObjectChecksumStorageService _storageService;
    private readonly ChecksumStorageFileCacheFactory _cache;

    public ObjectChecksumStorageProvider(
        ObjectChecksumStorageService storageService,
        ChecksumStorageFileCacheFactory cache)
    {
        _storageService = storageService;
        _cache = cache;
    }

    public Task<IEnumerable<ChecksumStorageListItem>> GetStorages()
    {
        return _storageService.GetAllItems();
    }

    public async Task<IChecksumStorage?> GetStorage(string id)
    {
        var entity = await _storageService.FindEntityById(id);
        if (entity == null)
            return null;

        var config = new ObjectStorageConfig
        (
            AccessKey: entity.AccessKey,
            SecretKey: entity.SecretKey,
            BucketName: entity.BucketName,
            Prefix: entity.Prefix,
            ServiceEndpoint: entity.ServiceEndpoint,
            PublicEndpoint: entity.PublicEndpoint
        );
        
        var storage = new ObjectChecksumStorage(config);
        var cache = _cache.Create($"ObjectChecksumStorage.{id}");
        return new CacheChecksumStorage(storage, cache);
    }
}