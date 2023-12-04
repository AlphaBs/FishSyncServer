namespace AlphabetUpdateServer.Entities;

public class UserEntity
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public virtual ICollection<BucketEntity> Buckets { get; set; } = new List<BucketEntity>();
}