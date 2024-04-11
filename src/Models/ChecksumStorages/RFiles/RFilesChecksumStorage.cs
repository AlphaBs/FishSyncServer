using System.Text.Json;
using RFiles.NET;

namespace AlphabetUpdateServer.Models.ChecksumStorages.RFiles;

public class RFilesChecksumStorage : IChecksumStorage
{
    private readonly RFilesClient _rClient;

    public RFilesChecksumStorage(
        string host, 
        bool isReadOnly,
        HttpClient httpClient)
    {
        Host = host;
        IsReadOnly = isReadOnly;
        _rClient = new RFilesClient(host, httpClient, JsonSerializerOptions.Default);
    }

    public bool IsReadOnly { get; private set; }
    public string Host { get; }

    public async IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        var objects = await _rClient.GetAllObjects();
        var locations = objects.Select(toFileLocation);
        foreach (var location in locations)
        {
            yield return location;
        }
    }

    public async IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums)
    {
        var objects = await _rClient.Query(checksums);
        var locations = objects.Select(toFileLocation);
        foreach (var location in locations)
        {
            yield return location;
        }
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var syncResult = await _rClient.Sync(checksums);

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

        var actions = syncResult.Uploads.Select(request =>
            new SyncAction
            (
                Type: "Http",
                Parameters: new Dictionary<string, string>()
            ));

        return new ChecksumStorageSyncResult(files.ToList(), actions.ToList());
    }

    private ChecksumStorageFile toFileLocation(RFilesObjectMetadata metadata) =>
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