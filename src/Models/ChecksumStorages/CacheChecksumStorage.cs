namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CacheChecksumStorage : IChecksumStorage
{
    private readonly IChecksumStorage _checksumStorage;
    private readonly ChecksumStorageFileCache _cache;

    public CacheChecksumStorage(
        IChecksumStorage checksumStorage, 
        ChecksumStorageFileCache cache)
    {
        _checksumStorage = checksumStorage;
        _cache = cache;
    }
    
    public bool IsReadOnly => _checksumStorage.IsReadOnly;
    
    public async IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        var files = _checksumStorage.GetAllFiles();
        await foreach (var file in files)
        {
            await _cache.SetFile(file);
            yield return file;
        }
    }

    public async Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums)
    {
        var foundFiles = new List<ChecksumStorageFile>();
        var notFoundChecksums = new HashSet<string>();
        
        foreach (var checksum in checksums)
        {
            var cachedFile = await _cache.GetFile(checksum);
            if (cachedFile is not null)
            {
                foundFiles.Add(cachedFile);
            }
            else
            {
                notFoundChecksums.Add(checksum);
            }
        }
        
        var result = await _checksumStorage.Query(notFoundChecksums);
        foreach (var file in result.FoundFiles)
        {
            foundFiles.Add(file);
            await _cache.SetFile(file);
        }
        
        return new ChecksumStorageQueryResult(foundFiles, result.NotFoundChecksums);
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var foundFiles = new Dictionary<string, ChecksumStorageFile>();
        var notFoundChecksums = new HashSet<string>();
        
        foreach (var checksum in checksums)
        {
            var cachedFile = await _cache.GetFile(checksum);
            if (cachedFile is not null)
            {
                foundFiles[checksum] = cachedFile;
            }
            else
            {
                notFoundChecksums.Add(checksum);
            }
        }
        
        var result = await _checksumStorage.Sync(notFoundChecksums);
        foreach (var file in result.SuccessFiles)
        {
            foundFiles[file.Checksum] = file;
            await _cache.SetFile(file);
        }
        
        return new ChecksumStorageSyncResult(
            foundFiles.Values, 
            result.RequiredActions);
    }
}