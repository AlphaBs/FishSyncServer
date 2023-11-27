using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Repositories;

namespace AlphabetUpdateServer.Models;

public class Bucket : IBucket
{
    private readonly IBucketFileRepository _fileRepository;
    private readonly IFileChecksumRepository _checksumRepository;

    public Bucket(
        string id,
        DateTime lastUpdated,
        IBucketFileRepository fileRepository,
        IFileChecksumRepository checksumRepository)
    {
        Id = id;
        LastUpdated = lastUpdated;
        _fileRepository = fileRepository;
        _checksumRepository = checksumRepository;
    }

    public string Id { get; }
    public DateTime LastUpdated { get; set; }

    public ValueTask ClearFiles() =>
        _fileRepository.UpdateFiles(Id, Enumerable.Empty<BucketFileEntity>());

    public async ValueTask<IEnumerable<BucketFile>> GetFiles()
    {
        var files = await _fileRepository.GetFiles(Id);
        var filesArr = files.ToArray();

        var locations = await _checksumRepository.BulkFind(filesArr.Select(file => file.Checksum));
        return filesArr.Zip(locations, (file, location) => new BucketFile(
            path: file.Path,
            size: file.Size,
            lastUpdated: file.LastUpdated,
            location: location.Location ?? string.Empty,
            checksum: file.Checksum
        ));
    }

    public async ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> files)
    {
        var filesArr = files.ToArray();
        var locations = await _checksumRepository.BulkFind(filesArr.Select(file => file.Checksum));

        var actions = new List<BucketSyncAction>();
        for (int i = 0; i < filesArr.Length; i++)
        {
            var file = filesArr[i];
            if (string.IsNullOrEmpty(locations[i].Checksum))
            {
                var action = new BucketSyncAction(file)
                {
                    ActionType = BucketSyncActionTypes.Http,
                    Parameters = new Dictionary<string, string>()
                };
                actions.Add(action);
            }
        }

        if (actions.Any())
        {
            return BucketSyncResult.ActionRequired(actions);
        }
        else
        {
            var now = DateTime.UtcNow;
            var entities = filesArr.Select(file => new BucketFileEntity
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