using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Models;

public record FileMetadata(
    long Size,
    DateTimeOffset LastUpdated,
    [property:MaxLength(64)] string Checksum);