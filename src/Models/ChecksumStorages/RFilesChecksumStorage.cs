using System.Text.Json;
using RFiles.NET;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class RFilesChecksumStorage : IChecksumStorage
{
    private readonly RFilesClient _rClient;

    public RFilesChecksumStorage(
        string host, 
        string? clientSecret,
        bool isReadOnly,
        HttpClient httpClient)
    {
        IsReadOnly = isReadOnly;
        _rClient = new RFilesClient(host, httpClient, JsonSerializerOptions.Default)
        {
            ClientSecret = clientSecret
        };
    }

    public bool IsReadOnly { get; }

    public async Task<IEnumerable<ChecksumStorageFile>> GetAllFiles()
    {
        var objects = await _rClient.GetAllObjects();
        return objects.Select(toChecksumStorageFile);
    }

    public async Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        if (!checksumSet.Any())
        {
            return new ChecksumStorageQueryResult([], []);
        }
        
        var objects = await _rClient.Query(checksumSet);
        var files = new List<ChecksumStorageFile>();
        foreach (var obj in objects)
        {
            checksumSet.Remove(obj.Hash);
            var file = toChecksumStorageFile(obj);
            files.Add(file);
        }

        return new ChecksumStorageQueryResult(files, checksumSet);
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        if (!checksumSet.Any())
        {
            return new ChecksumStorageSyncResult([], []);
        }
        var syncResult = await _rClient.Sync(checksumSet);

        var files = syncResult.Objects.Select(obj => 
            new ChecksumStorageFile
            (
                Checksum: obj.Hash, 
                Location: _rClient.GetObjectUri(obj.Hash).ToString(),
                Metadata: new FileMetadata
                (
                    Size: obj.Size,
                    Checksum: obj.Hash,
                    LastUpdated: obj.Uploaded
                )
            ));

        var actions = syncResult.Uploads.Select(createSyncAction);
        return new ChecksumStorageSyncResult(files.ToList(), actions.ToList());
    }

    private ChecksumStorageSyncAction createSyncAction(RFilesUploadRequest request)
    {
        var parameters = new Dictionary<string, string>()
        {
            ["method"] = request.Method,
            ["url"] = request.Url,
        };
        
        foreach (var kv in request.Headers)
        {
            parameters["headers." + kv.Key] = kv.Value;
        }

        return new ChecksumStorageSyncAction
        (
            Checksum: request.Hash,
            Action: new SyncAction
            (
                Type: "Http",
                Parameters: parameters
            )
        );
    }

    private ChecksumStorageFile toChecksumStorageFile(RFilesObjectMetadata metadata) =>
        new ChecksumStorageFile
        (
            Checksum: metadata.Hash,
            Location: _rClient.GetObjectUri(metadata.Hash).ToString(),
            Metadata: new FileMetadata
            (
                Size: metadata.Size,
                LastUpdated: metadata.Uploaded,
                Checksum: metadata.Hash
            )
        );
}