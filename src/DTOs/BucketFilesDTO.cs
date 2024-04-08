using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.DTOs;

public class BucketFilesDTO
{
    public string? Id { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public BucketFile[] Files { get; set; } = [];
}