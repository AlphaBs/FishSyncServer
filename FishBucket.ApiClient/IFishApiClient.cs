namespace FishBucket.ApiClient;

public interface IFishApiClient
{
    Task<FishBucketFilesResponse> GetBucketFiles(string id, CancellationToken cancellationToken = default);
}