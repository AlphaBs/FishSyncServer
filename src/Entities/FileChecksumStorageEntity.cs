namespace AlphabetUpdateServer.Entities;

public class FileChecksumStorageEntity
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsReadonly { get; set; }
}