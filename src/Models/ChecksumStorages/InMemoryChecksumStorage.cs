
namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class InMemoryChecksumStorage : IChecksumStorage
{
    private readonly Dictionary<string, ChecksumStorageFile> _storage = new();

    public bool IsReadOnly { get; set; }

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

    public Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var files = new List<ChecksumStorageFile>();
        var actions = new List<ChecksumStorageSyncAction>();

        foreach (var checksum in checksums)
        {
            if (_storage.TryGetValue(checksum, out var location))
            {
                files.Add(location);
            }
            else
            {
                var action = new SyncAction("InMemoryChecksumStorage", null);
                actions.Add(new ChecksumStorageSyncAction(checksum, action));
            }
        }

        var result = new ChecksumStorageSyncResult(files, actions);
        return Task.FromResult(result);
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