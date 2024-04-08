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

    public SyncAction CreateSyncAction(string checksum)
    {
        return Storages.First(storage => !storage.IsReadOnly).CreateSyncAction(checksum);
    }

    public async IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        foreach (var storage in Storages)
        {
            await foreach (var file in storage.GetAllFiles())
            {
                yield return file;
            }
        }
    }

    public async IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums)
    {
        var checksumSet = new HashSet<string>(checksums);
        foreach (var storage in Storages)
        {
            if (!checksumSet.Any())
                break;

            await foreach (var file in storage.Query(checksumSet))
            {
                checksumSet.Remove(file.Metadata.Checksum);
                yield return file;
            }
        }
    }
}