using System.Text.Json.Serialization;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Services.Buckets;

public record BucketMetadata(
    string Id,
    string Type,
    DateTimeOffset LastUpdated,
    BucketLimitations Limitations,
    IEnumerable<string> Owners);