using System.Text.Json.Serialization;

namespace FishBucket;

public class BucketSyncFile
{
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("checksum")]
    public string? Checksum { get; set; }
}