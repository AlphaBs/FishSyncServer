namespace AlphabetUpdateServer.DTOs;

public class BucketListResponse
{
    public IAsyncEnumerable<string>? Buckets { get; set; }
}