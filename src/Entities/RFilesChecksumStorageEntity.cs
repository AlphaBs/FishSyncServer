namespace AlphabetUpdateServer.Entities;

public class RFilesChecksumStorageEntity : ChecksumStorageEntity
{
    public static readonly string RFilesType = "rfiles";

    public RFilesChecksumStorageEntity()
    {
        Type = RFilesType;
    }

    public string Host { get; set; } = null!;
    public string? ClientSecret { get; set; }
}