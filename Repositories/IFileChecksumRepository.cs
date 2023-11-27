using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Repositories;

public interface IFileChecksumRepository
{
    ValueTask<IEnumerable<FileChecksumEntity>> GetAllFiles();
    ValueTask<IEnumerable<FileChecksumEntity>> GetAllFilesFromRepository(string repository);
    ValueTask<IEnumerable<FileChecksumEntity>> Find(string checksum);
    ValueTask<FileChecksumEntity?> Find(string checksum, string repository);
    ValueTask<FileChecksumEntity?> FindBest(string checksum);
    ValueTask<FileChecksumEntity[]> BulkFind(IEnumerable<string> checksums);
    ValueTask Add(FileChecksumEntity entity);
    ValueTask Remove(string checksum);
    ValueTask Remove(string checksum, string repository);
}