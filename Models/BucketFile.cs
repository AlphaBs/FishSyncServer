namespace AlphabetUpdateServer.Models;

public class BucketFile
{
    public BucketFile(
        string path,
        long size,
        DateTime lastUpdated,
        string location,
        string checksum) =>
        (Path, Size, LastUpdated, Location, Checksum) = 
        (path, size, lastUpdated, location, checksum);

    public string Path { get; set; }
    public long Size { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Location { get; set; }
    public string Checksum { get; set; }
}