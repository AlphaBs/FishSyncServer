namespace AlphabetUpdateServer.Models;

public class FileLocation
{
    public FileLocation(
        string checksum, 
        string storage,
        long size, 
        string location) =>
        (Checksum, Size, Location, Storage) = 
        (checksum, size, location, storage);

    public string Checksum { get; private set; }
    public string Storage { get; private set; }
    public long Size { get; private set; }
    public string Location { get; private set; }
}