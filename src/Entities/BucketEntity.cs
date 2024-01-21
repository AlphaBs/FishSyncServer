using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Entities;

public class BucketEntity
{
    public string Id { get; set; } = null!;
    public BucketLimitations Limitations { get; set; } = null!;
    public virtual ICollection<User> Owners { get; set; } = new List<User>();
    public virtual ICollection<BucketFile> Files { get; set; } = new List<BucketFile>();
    public DateTime LastUpdated { get; set; }
}