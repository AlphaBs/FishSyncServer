using System.Text.Json.Serialization;

namespace AlphabetUpdateServer.Entities;

[JsonDerivedType(typeof(RFilesChecksumStorageEntity), typeDiscriminator: RFilesChecksumStorageEntity.TypeName)]
public class FileChecksumStorageEntity
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsReadyOnly { get; set; }

    public virtual string GetEntityType()
    {
        return Type;
    }
}