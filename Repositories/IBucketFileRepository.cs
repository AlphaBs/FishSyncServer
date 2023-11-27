using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Repositories;

public interface IBucketFileRepository
{
    ValueTask<IEnumerable<BucketFileEntity>> GetFiles(string bucketId);

    // transaction
    ValueTask UpdateFiles(string bucketId, IEnumerable<BucketFileEntity> files);
}