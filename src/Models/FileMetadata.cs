namespace AlphabetUpdateServer.Models;

public record FileMetadata(
    long Size,
    DateTimeOffset LastUpdated,
    string Checksum);