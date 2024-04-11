using AlphabetUpdateServer.Models.Buckets;
using System.Text.Json.Serialization;

namespace AlphabetUpdateServer.DTOs;

public class BucketSyncRequestDTO
{
    [JsonPropertyName("files")]
    public List<BucketSyncFile>? Files { get; set; }
}