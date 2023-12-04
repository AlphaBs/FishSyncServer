namespace AlphabetUpdateServer.Models.Buckets;

public interface IBucketFactory
{
    ValueTask<IBucket> Create(string id, DateTime lastUpdated);
}