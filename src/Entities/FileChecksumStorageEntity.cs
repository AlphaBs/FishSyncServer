using System.Text.Json.Serialization;

namespace AlphabetUpdateServer.Entities;

[JsonDerivedType(typeof(RFilesChecksumStorageEntity), typeDiscriminator: RFilesChecksumStorageEntity.TypeName)]
public class FileChecksumStorageEntity
{
    public string? BucketId { get; set; }
    public virtual BucketEntity? Bucket { get; set; }
    public string StorageId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsReadyOnly { get; set; }
    public int Priority { get; set; }

    public virtual string GetEntityType()
    {
        return Type;
    }
}