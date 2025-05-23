using FishBucket.ChecksumStorages.Storages;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class ObjectChecksumStorageProvider : IChecksumStorageProvider
{
    private readonly ObjectChecksumStorageService _storageService;

    public ObjectChecksumStorageProvider(ObjectChecksumStorageService storageService)
    {
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

        var config = new ObjectStorageConfig
        (
            AccessKey: entity.AccessKey,
            SecretKey: entity.SecretKey,
            BucketName: entity.BucketName,
            Prefix: entity.Prefix,
            ServiceEndpoint: entity.ServiceEndpoint,
            PublicEndpoint: entity.PublicEndpoint
        );
        
        return new ObjectChecksumStorage(config);
    }
}