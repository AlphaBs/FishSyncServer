using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Entities;

[PrimaryKey(nameof(Checksum), nameof(Repository))]
public class FileChecksumEntity
{
    public string Checksum { get; set; } = null!;
    public string Repository { get; set; } = null!;
    public string Location { get; set; } = null!;
}