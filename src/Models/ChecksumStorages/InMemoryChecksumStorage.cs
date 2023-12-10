using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class InMemoryChecksumStorage : IFileChecksumStorage
{
    private readonly Dictionary<string, FileLocation> _innerStorage = new();
    public bool IsReadOnly { get; set; } = false;
    public Func<string, BucketSyncAction>? SyncActionFactory { get; set; }

    public BucketSyncAction CreateSyncAction(string checksum)
    {
        if (SyncActionFactory == null)
            throw new InvalidOperationException();
        else
            return SyncActionFactory(checksum);
    }

    public IAsyncEnumerable<FileLocation> GetAllFiles() => 
        internalGetAllFiles().ToAsyncEnumerable();

    private IEnumerable<FileLocation> internalGetAllFiles()
    {
        foreach (var item in _innerStorage.Values)
        {
            yield return item;
        }
    }

    public IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums) =>
        internalQuery(checksums).ToAsyncEnumerable();

    private IEnumerable<FileLocation> internalQuery(IEnumerable<string> checksums)
    {
        foreach (var checksum in checksums)
        {
            if (_innerStorage.TryGetValue(checksum, out var location))
            {
                yield return location;
            }
        }
    }

    public void Add(FileLocation fileLocation)
    {
        _innerStorage[fileLocation.Checksum] = fileLocation;
    }
}