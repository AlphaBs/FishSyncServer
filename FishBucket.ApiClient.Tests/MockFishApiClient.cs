namespace FishBucket.ApiClient.Tests;

public class MockFishApiClient : IFishApiClient
{
    private readonly Dictionary<string, FishBucketFilesResponse> _dict = new();

    public void Add(FishBucketFilesResponse bucket)
    {
        if (string.IsNullOrEmpty(bucket.Id))
            throw new ArgumentException("Id was null or empty");
        _dict[bucket.Id] = bucket;
    }
    
    public Task<FishBucketFilesResponse> GetBucketFiles(string id, CancellationToken cancellationToken = default)
    {
        var bucket = _dict[id];
        return Task.FromResult(bucket);
    }
}