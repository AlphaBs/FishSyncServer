
namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CompositeChecksumStorage : IChecksumStorage
{
    private readonly List<IChecksumStorage> _storages = new();

    public bool IsReadOnly => 
        !Storages.Any() || Storages.All(storage => storage.IsReadOnly);
    public IEnumerable<IChecksumStorage> Storages => _storages;

    public void AddStorage(IChecksumStorage storage)
    {
        _storages.Add(storage);
    }

    public async Task<IEnumerable<ChecksumStorageFile>> GetAllFiles()
    {
        var result = Enumerable.Empty<ChecksumStorageFile>();
        foreach (var storage in Storages)
        {
            var files = await storage.GetAllFiles();
            result = result.Concat(files);
        }

        return result;
    }

    public async Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        var foundFiles = new List<ChecksumStorageFile>();
        foreach (var storage in Storages)
        {
            if (!checksumSet.Any())
                break;

            var currentResult = await storage.Query(checksumSet);
            foreach (var file in currentResult.FoundFiles)
            {
                checksumSet.Remove(file.Metadata.Checksum);
                foundFiles.Add(file);
            }
        }

        return new ChecksumStorageQueryResult(foundFiles, checksumSet);
    }

    public Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        return Storages.First(storage => !storage.IsReadOnly).Sync(checksums);
    }
}