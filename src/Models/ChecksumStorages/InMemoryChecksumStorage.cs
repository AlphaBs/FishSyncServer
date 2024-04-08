namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class InMemoryChecksumStorage : IChecksumStorage
{
    private readonly Dictionary<string, ChecksumStorageFile> _storage = new();

    public bool IsReadOnly { get; set; }
    public Func<string, SyncAction>? SyncActionFactory { get; set; }

    public SyncAction CreateSyncAction(string checksum)
    {
        if (IsReadOnly || SyncActionFactory == null)
            throw new InvalidOperationException();
        return SyncActionFactory.Invoke(checksum);
    }

    public IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        return _storage.Values.ToAsyncEnumerable();
    }

    public IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums)
    {
        return query(checksums).ToAsyncEnumerable();
    }

    private IEnumerable<ChecksumStorageFile> query(IEnumerable<string> checksums)
    {
        foreach (var checksum in checksums)
        {
            if (_storage.TryGetValue(checksum, out var location))
            {
                yield return location;
            }
        }
    }

    public void Add(ChecksumStorageFile location)
    {
        _storage[location.Metadata.Checksum] = location;
    }

    public void AddRange(IEnumerable<ChecksumStorageFile> locations)
    {
        foreach (var location in locations)
        {
            Add(location);
        }
    }
}