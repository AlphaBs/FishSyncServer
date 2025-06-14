using AlphabetUpdateServer.Services.Buckets;
using FishBucket.ApiClient;

namespace AlphabetUpdateServer.Services;

public class LocalFishApiClient : IFishApiClient
{
    private readonly BucketFilesCacheService _bucketService;

    public LocalFishApiClient(BucketFilesCacheService bucketService)
    {
        _bucketService = bucketService;
    }

    public async Task<FishBucketFilesResponse> GetBucketFiles(string id, CancellationToken cancellationToken = default)
    {
        var bucket = await _bucketService.GetOrCreate(id, cancellationToken);
        return new FishBucketFilesResponse
        {
            Id = bucket.Id,
            Dependencies = bucket.Dependencies.ToList(),
            LastUpdated = bucket.LastUpdated,
            Files = bucket.Files.ToList()
        };
    }
}