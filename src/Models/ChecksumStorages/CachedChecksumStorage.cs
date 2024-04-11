namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class CachedChecksumStorage : IChecksumStorage
{
    private enum CacheStates
    {
        Valid,
        NotFound,
        Expired
    }

    private readonly IChecksumStorage _storage;
    private readonly IChecksumStorageFileCacheRepository _repository;
    private readonly TimeSpan _maxCacheAge;

    public CachedChecksumStorage(
        IChecksumStorage storage,
        TimeSpan maxCacheAge,
        IChecksumStorageFileCacheRepository repository) =>
        (_storage, _repository, _maxCacheAge) =
        (storage, repository, maxCacheAge);

    public bool IsReadOnly => _storage.IsReadOnly;

    public async IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        // 캐시된 데이터 먼저 반환
        var cacheStates = new Dictionary<string, CacheStates>();
        foreach (var cache in await _repository.GetAllCaches())
        {
            if (isExpiredCache(cache))
                cacheStates[cache.Checksum] = CacheStates.Expired;
            else
            {
                if (cache.TryGetLocation(out var location) && cache.TryGetMetadata(out var metadata))
                {
                    cacheStates[cache.Checksum] = CacheStates.Valid;
                    yield return new ChecksumStorageFile(cache.Checksum, location, metadata);
                }
            }
        }

        // 내부 스토리지 쿼리
        var newFiles = new List<ChecksumStorageFile>();
        await foreach (var file in _storage.GetAllFiles())
        {
            newFiles.Add(file);
            yield return file;
        }

        // 캐시 갱신
        foreach (var newFile in newFiles)
        {
            var cache = ChecksumStorageFileCache.CreateExistentCache(
                newFile.Metadata.Checksum,
                newFile.Location,
                newFile.Metadata);

            if (cacheStates.TryGetValue(newFile.Metadata.Checksum, out var state))
            {
                if (state == CacheStates.Expired)
                    _repository.UpdateCache(cache);

                cacheStates.Remove(newFile.Metadata.Checksum);
            }
            else
            {
                _repository.AddCache(cache);
            }
        }

        // 남은 캐시 삭제
        _repository.RemoveCaches(cacheStates.Keys);
        await _repository.SaveChanges();
    }

    public async IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums)
    {
        // 캐시에서 찾을 수 있는 것 먼저 처리
        var cacheStates = checksums.ToDictionary(checksum => checksum, _ => CacheStates.NotFound);
        var caches = await _repository.Query(cacheStates.Keys);
        foreach (var cache in caches)
        {
            if (isExpiredCache(cache))
            {
                cacheStates[cache.Checksum] = CacheStates.Expired;
            }
            else
            {
                if (cache.TryGetLocation(out var location) && cache.TryGetMetadata(out var metadata))
                    yield return new ChecksumStorageFile(cache.Checksum, location, metadata);

                // 만료되지 않은 캐시를 발견한 경우 재질의 하지 않음
                cacheStates.Remove(cache.Checksum);
            }
        }

        // 캐시에서 찾을 수 없는 체크섬이 있는 경우, 내부 스토리지에 다시 쿼리
        if (cacheStates.Any())
        {
            var newFiles = new List<ChecksumStorageFile>(cacheStates.Count);
            await foreach (var file in _storage.Query(cacheStates.Keys))
            {
                yield return file;
                newFiles.Add(file);
            }

            // 내부에서 발견한 파일은 캐시
            foreach (var newFile in newFiles)
            {
                var cache = ChecksumStorageFileCache.CreateExistentCache(
                    newFile.Metadata.Checksum,
                    newFile.Location,
                    newFile.Metadata);

                if (cacheStates.TryGetValue(newFile.Metadata.Checksum, out var state) && state == CacheStates.Expired)
                    _repository.UpdateCache(cache);
                else
                    _repository.AddCache(cache);

                cacheStates.Remove(newFile.Metadata.Checksum);
            }

            // 발견하지 못한 파일도 캐시
            foreach (var remainCache in cacheStates)
            {
                var cache = ChecksumStorageFileCache.CreateNonExistentCache(remainCache.Key);
                if (remainCache.Value == CacheStates.Expired)
                    _repository.UpdateCache(cache);
                else
                    _repository.AddCache(cache);
            }

            await _repository.SaveChanges();
        }
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        // Sync 작업은 캐시된 데이터를 이용하지 않음
        var syncResult = await _storage.Sync(checksums);

        // 응답된 데이터 캐시
        var files = syncResult.SuccessFiles;
        _repository.RemoveCaches(files.Select(f => f.Checksum));
        foreach (var file in files)
        {
            var cache = ChecksumStorageFileCache.CreateExistentCache(file.Checksum, file.Location, file.Metadata);
            _repository.AddCache(cache);
        }
        await _repository.SaveChanges();

        return syncResult;
    }

    private bool isExpiredCache(ChecksumStorageFileCache cache)
    {
        return (DateTimeOffset.UtcNow - cache.CachedAt) > _maxCacheAge;
    }
}