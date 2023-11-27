namespace AlphabetUpdateServer.Repositories;

public class LocalFileRepository
{
    public LocalFileRepository(string path)
    {
        RootPath = path;
    }

    public string RootPath { get; }

    public Uri? FindLocationByChecksum(string checksum)
    {
        var path = Path.Combine(RootPath, checksum);
        if (File.Exists(path))
            return new Uri(path);
        else
            return null;
    }

    public void AddLocation(string checksum, Uri location)
    {
        return;
    }
}