using AlphabetUpdateServer.Models;

namespace AlphabetUpdateServer.Repositories;

public interface ICachedFileChecksumRepository
{
    IAsyncEnumerable<FileLocation> GetAllFiles();
    IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums);
    IAsyncEnumerable<FileLocation> Find(string checksum);
    ValueTask<FileLocation?> Find(string checksum, string storage);
    ValueTask<FileLocation?> FindBest(string checksum);
    ValueTask Add(FileLocation entity);
    ValueTask Add(IEnumerable<FileLocation> entities);
    ValueTask Remove(string checksum);
    ValueTask Remove(string checksum, string storage);
}