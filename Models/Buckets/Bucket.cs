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
        DateTime lastUpdated,
        IBucketFileRepository fileRepository,
        IFileChecksumStorage checksumStorage)
    {
        Id = id;
        LastUpdated = lastUpdated;
        _fileRepository = fileRepository;
        _checksumStorage = checksumStorage;
    }

    public string Id { get; }
    public DateTime LastUpdated { get; set; }

    public ValueTask ClearFiles() =>
        _fileRepository.UpdateFiles(Id, Enumerable.Empty<BucketFileEntity>());

    public async IAsyncEnumerable<BucketFile> GetFiles()
    {
        var files = await _fileRepository.GetFiles(Id);
        var checksumFileMap = files.ToDictionary(f => f.Checksum, f => f);

        var checksumLocations = _checksumStorage.Query(checksumFileMap.Keys);
        await foreach (var checksumLocation in checksumLocations)
        {
            var fileEntity = checksumFileMap[checksumLocation.Checksum];
            var bucketFile = new BucketFile(
                path: fileEntity.Path,
                size: fileEntity.Size,
                lastUpdated: fileEntity.LastUpdated,
                location: checksumLocation.Location ?? string.Empty,
                checksum: fileEntity.Checksum
            );
            yield return bucketFile;
        }
    }

    public async ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> files)
    {
        Dictionary<string, BucketSyncFile> requestChecksumFileMap = files
            .Where(f => !string.IsNullOrEmpty(f.Checksum))
            .ToDictionary(f => f.Checksum!, f => f);
            
        var queryFiles = _checksumStorage.Query(requestChecksumFileMap.Keys);
        await foreach (var queryFile in queryFiles)
        {
            if (requestChecksumFileMap.TryGetValue(queryFile.Checksum, out var requestFile))
            {
                if (requestFile.Size != queryFile.Size)
                {
                    var validationAction = new FileSizeValidationAction(requestFile);
                    return BucketSyncResult.ActionRequired(
                        new BucketSyncAction[] { validationAction });
                }
                requestChecksumFileMap.Remove(queryFile.Checksum);
            }
        }

        var actions = new List<BucketSyncAction>();
        foreach (var remainHash in requestChecksumFileMap.Keys)
        {
            var action = _checksumStorage.CreateSyncAction(remainHash);
            actions.Add(action);
        }

        if (actions.Any())
        {
            return BucketSyncResult.ActionRequired(actions);
        }
        else
        {
            var now = DateTime.UtcNow;
            var entities = requestChecksumFileMap.Values.Select(file => new BucketFileEntity
            {
                BucketId = Id,
                Path = file.Path,
                Size = file.Size,
                LastUpdated = now,
                Checksum = file.Checksum
            });
            await _fileRepository.UpdateFiles(Id, entities);
            return BucketSyncResult.Success();
        }
    }
}