namespace AlphabetUpdateServer.Entities;

public class RFilesChecksumStorageEntity : FileChecksumStorageEntity
{
    public const string TypeName = "rfiles";
    
    public string Host { get; set; } = null!;
    public string? ClientSecret { get; set; }

    public override string GetEntityType()
    {
        return TypeName;
    }
}