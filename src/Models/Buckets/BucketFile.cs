namespace AlphabetUpdateServer.Models.Buckets;

public record BucketFile(
    string Path, 
    long Size, 
    DateTimeOffset LastUpdated, 
    string? Location, 
    string Checksum);