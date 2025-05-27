using System.Text.Json.Serialization;

namespace FishBucket;

public record FileMetadata(
    [property:JsonPropertyName("size")]
    long Size,
    [property:JsonPropertyName("lastUpdated")]
    DateTimeOffset LastUpdated,
    [property:JsonPropertyName("checksum")]
    string Checksum);