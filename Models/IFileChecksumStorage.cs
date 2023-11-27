namespace AlphabetUpdateServer.Models;

public interface IFileChecksumStorage
{
    string? GetFileLocation(string checksum);
    Dictionary<string, string> CreateParametersForUpload(string checksum);
}