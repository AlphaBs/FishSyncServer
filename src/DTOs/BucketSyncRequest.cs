using System.Text.Json.Serialization;
using FishBucket;

namespace AlphabetUpdateServer.DTOs;

public class BucketSyncRequest
{
    [JsonPropertyName("files")]
    public List<BucketSyncFile>? Files { get; set; }
}