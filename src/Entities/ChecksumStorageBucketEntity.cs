using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.AspNetCore.Identity;

namespace AlphabetUpdateServer.Entities;

public class ChecksumStorageBucketEntity
{
    public string Id { get; set; } = null!;
    public BucketLimitations Limitations { get; set; } = null!;
    public ICollection<User> Owners { get; } = [];
    public ICollection<BucketOwnerUserEntity> OwnerUserEntities { get; set; } = [];
    public ICollection<ChecksumStorageBucketFileEntity> Files { get; } = [];
    public DateTimeOffset LastUpdated { get; set; }
    public string ChecksumStorageId { get; set; } = null!;
}