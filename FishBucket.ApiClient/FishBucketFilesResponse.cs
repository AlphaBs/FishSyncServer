using System.Text.Json.Serialization;

namespace FishBucket.ApiClient;

public class FishBucketFilesResponse
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("lastUpdated")] public DateTimeOffset LastUpdated { get; set; }
    [JsonPropertyName("files")] public IReadOnlyCollection<BucketFile> Files { get; set; } = [];
    [JsonPropertyName("dependencies")] public IReadOnlyCollection<string> Dependencies { get; set; } = [];
}