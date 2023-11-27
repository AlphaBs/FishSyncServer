using AlphabetUpdateServer.Repositories;

namespace AlphabetUpdateServer.Models;

public class BucketFactory : IBucketFactory
{
    private readonly IBucketFileRepository _fileRepository;
    private readonly IFileChecksumRepository _checksumRepository;

    public BucketFactory(
        IBucketFileRepository fileRepository, 
        IFileChecksumRepository checksumRepository)
    {
        _fileRepository = fileRepository;
        _checksumRepository = checksumRepository;
    }


    public IBucket Create(string id, DateTime lastUpdated)
    {
        return new Bucket(id, lastUpdated, _fileRepository, _checksumRepository);
    }
}