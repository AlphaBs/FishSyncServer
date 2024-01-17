using AlphabetUpdateServer.Repositories;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class FileChecksumStorageManager : IFileChecksumStorageFactory
{
    private readonly ICachedFileChecksumRepository _cacheRepository;
    private readonly IFileChecksumStorageRepository _storageRepository;
    private readonly IEnumerable<IEntityToStorageConverter> _factoryCollection;

    public FileChecksumStorageManager(
        ICachedFileChecksumRepository cacheRepository,
        IFileChecksumStorageRepository storageRepository,
        IEnumerable<IEntityToStorageConverter> factoryCollection) =>
        (_cacheRepository, _storageRepository, _factoryCollection) = 
        (cacheRepository, storageRepository, factoryCollection);

    public async ValueTask<IFileChecksumStorage> CreateStorageForBucket(string bucketId)
    {
        var entities = await _storageRepository.GetStorages(bucketId);
        var composite = new CompositeFileChecksumStorage();

        foreach (var entity in entities)
        {
            foreach (var factory in _factoryCollection)
            {
                if (factory.CanCreate(entity))
                {
                    var storage = factory.Create(entity);
                    composite.AddStorage(storage);
                }
            }
        }

        var cached = new CachedFileChecksumStorage(composite, _cacheRepository);
        return cached;
    }
}