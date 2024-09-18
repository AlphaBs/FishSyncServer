using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class BucketOwnerUserEntity
{
    public string ChecksumStorageBucketEntityId { get; set; } = default!;
    public string UserId { get; set; } = default!;
}