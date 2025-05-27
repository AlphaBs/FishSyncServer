using System.Text.Json.Serialization;

namespace FishBucket;

public record SyncAction(
    [property:JsonPropertyName("type")] 
    string Type,
    
    [property:JsonPropertyName("parameters")] 
    IReadOnlyDictionary<string, string>? Parameters);