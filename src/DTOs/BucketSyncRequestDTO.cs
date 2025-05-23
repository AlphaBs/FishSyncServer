using System.Text.Json.Serialization;
using FishBucket;

namespace AlphabetUpdateServer.DTOs;

public class BucketSyncRequestDTO
{
    [JsonPropertyName("files")]
    public List<BucketSyncFile>? Files { get; set; }
}