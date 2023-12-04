using AlphabetUpdateServer.Repositories;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Models.Buckets;

public class BucketFactory : IBucketFactory
{
    private readonly IBucketFileRepository _fileRepository;
    private readonly IFileChecksumStorageManager _checksumStorageManager;

    public BucketFactory(
        IBucketFileRepository fileRepository, 
        IFileChecksumStorageManager checksumStorageManager)
    {
        _fileRepository = fileRepository;
        _checksumStorageManager = checksumStorageManager;
    }


    public async ValueTask<IBucket> Create(string id, DateTime lastUpdated)
    {
        var checksumStorage = await _checksumStorageManager.GetStorageForBucket(id);
        return new Bucket(id, lastUpdated, _fileRepository, checksumStorage);
    }
}