using System.Text.Json.Serialization;

namespace FishBucket.Alphabet;

public class UpdateFileCollection
{
    [JsonPropertyName("lastUpdate")]
    public DateTimeOffset LastUpdate { get; set; }

    [JsonPropertyName("hashAlgorithm")]
    public string? HashAlgorithm { get; set; }

    [JsonPropertyName("files")] public IEnumerable<UpdateFile> Files { get; set; } = [];
}