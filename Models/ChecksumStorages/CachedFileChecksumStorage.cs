using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Repositories;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CachedFileChecksumStorage : IFileChecksumStorage
{
    public bool IsReadOnly => _storage.IsReadOnly;

    private readonly IFileChecksumStorage _storage;
    private readonly ICachedFileChecksumRepository _caches;

    public CachedFileChecksumStorage(
        IFileChecksumStorage storage,
        ICachedFileChecksumRepository cacheRepository) =>
        (_storage, _caches) = (storage, cacheRepository);

    public BucketSyncAction CreateSyncAction(string checksum)
    {
        return _storage.CreateSyncAction(checksum);
    }

    public async IAsyncEnumerable<FileLocation> GetAllFiles()
    {
        // return cached files first
        var checksumSet = new HashSet<string>();
        await foreach (var file in _caches.GetAllFiles())
        {
            checksumSet.Add(file.Checksum);
            yield return new FileLocation(
                checksum: file.Checksum,
                storage: file.Storage,
                size: file.Size,
                location: file.Location);
        }

        // get from inner storage
        var newFiles = new List<FileLocation>();
        await foreach (var file in _caches.GetAllFiles())
        {
            if (!checksumSet.Remove(file.Checksum)) // if not cached file
            {
                newFiles.Add(file);
                yield return file;
            }
        }

        // bulk update not cached files
        if (newFiles.Any())
        {
            await _caches.Add(newFiles.Select(file => new FileLocation(
                checksum: file.Checksum,
                storage: file.Storage,
                size: file.Size,
                location: file.Location
            )));
        }
    }

    public async IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums)
    {
        // return cached files first
        var checksumSet = new HashSet<string>(checksums);
        await foreach (var file in _caches.Query(checksumSet))
        {
            checksumSet.Remove(file.Checksum);
            yield return new FileLocation(
                checksum: file.Checksum,
                storage: file.Storage,
                size: file.Size,
                location: file.Location);
        }

        // if there are not cached files
        if (checksumSet.Any())
        {
            // get from inner storage
            var newFiles = new List<FileLocation>();
            await foreach (var file in _storage.Query(checksumSet))
            {
                newFiles.Add(file);
                yield return file;
            }

            // bulk update not cached files
            await _caches.Add(newFiles.Select(file => new FileLocation(
                checksum: file.Checksum,
                storage: file.Storage,
                size: file.Size,
                location: file.Location
            )));
        }
    }
}