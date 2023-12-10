using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.ChecksumStorages;
using AlphabetUpdateServer.Repositories;

namespace AlphabetUpdateServer.Models.Buckets;

public class Bucket : IBucket
{
    private readonly IBucketFileRepository _fileRepository;
    private readonly IFileChecksumStorage _checksumStorage;

    public Bucket(
        string id,
        DateTimeOffset lastUpdated,
        IBucketFileRepository fileRepository,
        IFileChecksumStorage checksumStorage)
    {
        Id = id;
        LastUpdated = lastUpdated;
        _fileRepository = fileRepository;
        _checksumStorage = checksumStorage;
    }

    public string Id { get; }
    public DateTimeOffset LastUpdated { get; set; }

    public ValueTask ClearFiles() =>
        _fileRepository.UpdateFiles(Id, Enumerable.Empty<BucketFileEntity>());

    public async IAsyncEnumerable<BucketFile> GetFiles()
    {
        // IBucketFileRepository 는 파일의 경로와 체크섬 등 메타데이터만 가지고 있으며 실제 위치는 모름
        // IFileChecksumStorage 에서 파일의 실제 위치를 찾고 파일 목록을 반환함
        var files = await _fileRepository.GetFiles(Id);
        var checksumFileMap = files.ToDictionary(f => f.Checksum, f => f);

        // 찾아야 할 체크섬 전체를 질의
        var checksumLocations = _checksumStorage.Query(checksumFileMap.Keys);
        await foreach (var checksumLocation in checksumLocations)
        {
            var fileEntity = checksumFileMap[checksumLocation.Checksum];
            checksumFileMap.Remove(checksumLocation.Checksum);

            var bucketFile = new BucketFile(
                Path: fileEntity.Path,
                Size: fileEntity.Size,
                LastUpdated: fileEntity.LastUpdated,
                Location: checksumLocation.Location ?? string.Empty,
                Checksum: fileEntity.Checksum);
            yield return bucketFile;
        }

        // ChecksumStorage 에서 찾지 못한 파일은 Location 을 null 로 하여 반환
        foreach (var file in checksumFileMap.Values)
        {
            var bucketFile = new BucketFile(
                Path: file.Path,
                Size: file.Size,
                LastUpdated: file.LastUpdated,
                Location: null,
                Checksum: file.Checksum);
            yield return bucketFile;
        }
    }

    public async ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> files)
    {
        var (result, entities) = await BucketSyncProcessor.Sync(Id, _checksumStorage, files);

        if (result.IsSuccess)
        {
            await _fileRepository.UpdateFiles(Id, entities);
        }

        return result;
    }
}