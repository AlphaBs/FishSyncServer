using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CompositeFileChecksumStorage : IFileChecksumStorage
{
    public bool IsReadOnly => 
        !Storages.Any() || Storages.All(storage => storage.IsReadOnly);

    private readonly List<IFileChecksumStorage> _storages = new();
    public IEnumerable<IFileChecksumStorage> Storages => _storages;

    public void AddStorage(IFileChecksumStorage storage)
    {
        _storages.Add(storage);
    }

    public BucketSyncAction CreateSyncAction(BucketSyncFile file)
    {
        return Storages.First(storage => !storage.IsReadOnly).CreateSyncAction(file);
    }

    public async IAsyncEnumerable<FileLocation> GetAllFiles()
    {
        foreach (var storage in Storages)
        {
            await foreach (var file in storage.GetAllFiles())
            {
                yield return file;
            }
        }
    }

    public async IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums)
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