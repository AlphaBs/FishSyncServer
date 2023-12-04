using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Repositories;

public interface IBucketRepository
{
    ValueTask<IEnumerable<BucketEntity>> GetAllBuckets();
    ValueTask<BucketEntity?> FindBucketById(string bucketId);
    ValueTask<IEnumerable<BucketEntity>> FindBucketsByOwner(string ownerId);
}