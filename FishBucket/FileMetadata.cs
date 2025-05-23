using System.ComponentModel.DataAnnotations;

namespace FishBucket;

public record FileMetadata(
    long Size,
    DateTimeOffset LastUpdated,
    [property:MaxLength(64)] string Checksum);