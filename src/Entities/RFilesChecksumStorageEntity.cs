namespace AlphabetUpdateServer.Entities;

public class RFilesChecksumStorageEntity : FileChecksumStorageEntity
{
    public string Host { get; set; } = null!;
    public string? ClientSecret { get; set; }
}