namespace AlphabetUpdateServer.ViewModels.ChecksumStorages;

public class AddChecksumStorageViewModel
{
    public string? Id { get; set; }
    public bool IsReadonly { get; set; }
    public string? Host { get; set; }
    public string? ClientSecret { get; set; }
}