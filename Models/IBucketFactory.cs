namespace AlphabetUpdateServer.Models;

public interface IBucketFactory
{
    IBucket Create(string id, DateTime lastUpdated);
}