using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageBucketEntity
{
    public string Id { get; set; } = null!;
    public BucketLimitations Limitations { get; set; } = null!;
    public List<User> Owners { get; set; } = [];
    public List<BucketFileEntity> Files { get; set; } = [];
    public DateTimeOffset LastUpdated { get; set; }
    public string ChecksumStorageId { get; set; } = null!;
}