using System.Text.Json.Serialization;

namespace FishBucket.ApiClient;

public class FishBucketResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("lastUpdated")]
    public DateTimeOffset? LastUpdated { get; set; }

    [JsonPropertyName("limitations")]
    public BucketLimitations? Limitations { get; set; }

    [JsonPropertyName("files")] public IReadOnlyCollection<BucketFile> Files { get; set; } = [];
}