using AlphabetUpdateServer.Services;

namespace AlphabetUpdateServer.DTOs;

public class BucketListDTO
{
    public IReadOnlyCollection<string>? Buckets { get; set; }
}