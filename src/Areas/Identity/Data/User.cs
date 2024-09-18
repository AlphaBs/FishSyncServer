using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Entities;
using Microsoft.AspNetCore.Identity;

namespace AlphabetUpdateServer.Areas.Identity.Data;

public class User : IdentityUser
{
    [MaxLength(128)]
    public string? Discord { get; set; }

    public ICollection<ChecksumStorageBucketEntity> Buckets { get; set; } = [];
    public ICollection<BucketOwnerUserEntity> BucketOwnerUserEntities { get; set; } = [];
}