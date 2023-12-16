using AlphabetUpdateServer.Areas.Identity.Data;

namespace AlphabetUpdateServer.Entities;

public class BucketEntity
{
    public string Id { get; set; } = null!;
    public string? Status { get; set; }
    public virtual ICollection<User> Owners { get; set; } = new List<User>();
    public virtual ICollection<BucketFileEntity> Files { get; set; } = new List<BucketFileEntity>();
    public virtual ICollection<FileChecksumStorageEntity> Storages { get; set; } = new List<FileChecksumStorageEntity>();
    public DateTime LastUpdated { get; set; }
}