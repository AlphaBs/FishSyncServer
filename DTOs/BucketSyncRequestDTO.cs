using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.DTOs;

public class BucketSyncRequestDTO
{
    public IEnumerable<BucketSyncFile>? Files { get; set; }
}