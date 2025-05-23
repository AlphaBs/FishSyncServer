namespace AlphabetUpdateServer.DTOs;

public class BucketListDTO
{
    public IAsyncEnumerable<string>? Buckets { get; set; }
}