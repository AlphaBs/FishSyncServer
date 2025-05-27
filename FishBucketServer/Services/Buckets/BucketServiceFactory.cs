namespace AlphabetUpdateServer.Services.Buckets;

public class BucketServiceFactory
{
    private readonly IEnumerable<IBucketService> _buckets;

    public BucketServiceFactory(IEnumerable<IBucketService> buckets)
    {
        _buckets = buckets;
    }

    public IBucketService? GetService(string type)
    {
        return _buckets.FirstOrDefault(service => service.Type == type);
    }
    
    public IBucketService GetRequiredService(string type)
    {
        return _buckets.First(service => service.Type == type);
    }
}