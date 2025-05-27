using System.Text.Json.Serialization;

namespace FishBucket;

public record BucketFile(
    [property:JsonPropertyName("path")] 
    string Path,
    
    [property:JsonPropertyName("location")] 
    string? Location,
    
    [property:JsonPropertyName("metadata")] 
    FileMetadata Metadata);