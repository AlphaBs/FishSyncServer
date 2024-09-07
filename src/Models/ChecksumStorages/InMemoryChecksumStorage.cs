
namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class InMemoryChecksumStorage : IChecksumStorage
{
    private readonly Dictionary<string, ChecksumStorageFile> _storage = new();

    public bool IsReadOnly { get; set; }

    public IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        return _storage.Values.ToAsyncEnumerable();
    }

    public Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        var foundFiles = new List<ChecksumStorageFile>();
        var notFoundChecksums = new List<string>();
        foreach (var checksum in checksumSet)
        {
            if (_storage.TryGetValue(checksum, out var file))
            {
                foundFiles.Add(file);
            }
            else
            {
                notFoundChecksums.Add(checksum);
            }
        }
        var result = new ChecksumStorageQueryResult(foundFiles, notFoundChecksums);
        return Task.FromResult(result);
    }

    public Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        var files = new List<ChecksumStorageFile>();
        var actions = new List<ChecksumStorageSyncAction>();

        foreach (var checksum in checksumSet)
        {
            if (_storage.TryGetValue(checksum, out var file))
            {
                files.Add(file);
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

    public void Add(ChecksumStorageFile file)
    {
        _storage[file.Metadata.Checksum] = file;
    }

    public void AddRange(IEnumerable<ChecksumStorageFile> files)
    {
        foreach (var file in files)
        {
            Add(file);
        }
    }

    public void Clear()
    {
        _storage.Clear();
    }
}