using AlphabetUpdateServer.Services.ChecksumStorageCaches;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CacheChecksumStorage : IChecksumStorage
{
    private readonly string _id;
    private readonly IChecksumStorage _checksumStorage;
    private readonly IChecksumStorageCache _cache;

    public CacheChecksumStorage(
        string id,
        IChecksumStorage checksumStorage, 
        IChecksumStorageCache cache)
    {
        _id = id;
        _checksumStorage = checksumStorage;
        _cache = cache;
    }
    
    public bool IsReadOnly => _checksumStorage.IsReadOnly;
    
    public async Task<IEnumerable<ChecksumStorageFile>> GetAllFiles()
    {
        var files = (await _checksumStorage.GetAllFiles()).ToList();
        await _cache.SetFiles(_id, files);
        return files;
    }

    public async Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums)
    {
        var foundFiles = new List<ChecksumStorageFile>();
        var notFoundChecksums = new HashSet<string>();
        
        foreach (var checksum in checksums)
        {
            var cachedFile = await _cache.GetFile(_id, checksum);
            if (cachedFile is not null)
            {
                foundFiles.Add(cachedFile);
            }
            else
            {
                notFoundChecksums.Add(checksum);
            }
        }

        if (notFoundChecksums.Any())
        {
            var result = await _checksumStorage.Query(notFoundChecksums);
            await _cache.SetFiles(_id, result.FoundFiles);
            foundFiles.AddRange(result.FoundFiles);
            return new ChecksumStorageQueryResult(foundFiles, result.NotFoundChecksums);
        }
        else
        {
            return new ChecksumStorageQueryResult(foundFiles, []);
        }
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var foundFiles = new Dictionary<string, ChecksumStorageFile>();
        var notFoundChecksums = new HashSet<string>();
        
        foreach (var checksum in checksums)
        {
            var cachedFile = await _cache.GetFile(_id, checksum);
            if (cachedFile is not null)
            {
                foundFiles[checksum] = cachedFile;
            }
            else
            {
                notFoundChecksums.Add(checksum);
            }
        }

        if (notFoundChecksums.Any())
        {
            var result = await _checksumStorage.Sync(notFoundChecksums);
            await _cache.SetFiles(_id, result.SuccessFiles);
            foreach (var file in result.SuccessFiles)
            {
                foundFiles[file.Checksum] = file;
            }

            return new ChecksumStorageSyncResult(
                foundFiles.Values,
                result.RequiredActions);
        }
        else
        {
            return new ChecksumStorageSyncResult(foundFiles.Values, []);
        }
    }
}