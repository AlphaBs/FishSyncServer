namespace AlphabetUpdateServer.Entities;

public class ChecksumBucketEntity : BucketEntity
{
    public ICollection<FileChecksumStorageEntity> ChecksumStorages  { get; set; } = null!;
}