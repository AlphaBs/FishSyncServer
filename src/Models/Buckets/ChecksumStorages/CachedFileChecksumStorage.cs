using AlphabetUpdateServer.Models.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CachedFileChecksumStorage : IFileChecksumStorage
{
    public bool IsReadOnly => _storage.IsReadOnly;

    private readonly IFileChecksumStorage _storage;
    private readonly ApplicationDbContext _dbContext;

    public CachedFileChecksumStorage(
        IFileChecksumStorage storage,
        ApplicationDbContext dbContext) =>
        (_storage, _dbContext) = (storage, dbContext);

    public BucketSyncAction CreateSyncAction(BucketSyncFile syncFile)
    {
        return _storage.CreateSyncAction(syncFile);
    }

    public async IAsyncEnumerable<FileLocation> GetAllFiles()
    {
        // return cached files first
        var checksumSet = new HashSet<string>();
        await foreach (var file in getCachedFiles())
        {
            checksumSet.Add(file.Metadata.Checksum);
            yield return new FileLocation(file.Storage, file.Location, file.Metadata);
        }

        // get from inner storage
        var newFiles = new List<FileLocation>();
        await foreach (var file in _storage.GetAllFiles())
        {
            if (!checksumSet.Remove(file.Metadata.Checksum)) // if not cached file
            {
                newFiles.Add(file);
                yield return file;
            }
        }

        // bulk update not cached files
        if (newFiles.Any())
        {
            await addCaches(newFiles);
        }
    }

    private IAsyncEnumerable<FileLocation> getCachedFiles()
    {
        return _dbContext.ChecksumLocationCache.AsAsyncEnumerable();
    }

    public async IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums)
    {
        // return cached files first
        var checksumSet = new HashSet<string>(checksums);
        await foreach (var file in queryCaches(checksumSet))
        {
            checksumSet.Remove(file.Metadata.Checksum);
            yield return file;
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
            await addCaches(newFiles);
        }
    }

    private IAsyncEnumerable<FileLocation> queryCaches(IEnumerable<string> checksums)
    {
        return _dbContext.ChecksumLocationCache
            .Where(file => checksums.Contains(file.Metadata.Checksum))
            .AsAsyncEnumerable();
    }

    private async Task addCaches(IEnumerable<FileLocation> files)
    {
        await _dbContext.ChecksumLocationCache.AddRangeAsync(files);
    }
}