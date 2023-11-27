using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.DTOs;

public class BucketSyncRequestDTO
{
    public IEnumerable<BucketSyncFile>? Files { get; set; }
}