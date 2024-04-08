using System.Text.Json;
using AlphabetUpdateServer.Models.Buckets;
using RFiles.NET;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class RFilesChecksumStorage : IChecksumStorage
{
    private readonly RFilesClient _client;

    public RFilesChecksumStorage(
        string host, 
        bool isReadOnly,
        HttpClient httpClient)
    {
        Host = host;
        IsReadOnly = isReadOnly;
        _client = new RFilesClient(host, httpClient, JsonSerializerOptions.Default);
    }

    public bool IsReadOnly { get; private set; }
    public string Host { get; }

    public SyncAction CreateSyncAction(string checksum)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("ReadOnly storage");
        }

        return new SyncAction
        (
            Type: BucketSyncActionTypes.Http,
            Parameters: new Dictionary<string, string>()
            {

            }
        );
    }

    public async IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        var objects = await _client.GetAllObjects();
        var locations = objects.Select(toFileLocation);
        foreach (var location in locations)
        {
            yield return location;
        }
    }

    public async IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums)
    {
        var objects = await _client.Query(checksums);
        var locations = objects.Select(toFileLocation);
        foreach (var location in locations)
        {
            yield return location;
        }
    }

    private ChecksumStorageFile toFileLocation(RFilesObjectMetadata metadata) =>
        new ChecksumStorageFile
        (
            Checksum: metadata.Hash,
            Location: _client.GetObjectUri(metadata.Hash).ToString(),
            Metadata: new FileMetadata
            (
                Size: metadata.Size,
                LastUpdated: metadata.Uploaded,
                Checksum: metadata.Hash
            )
        );
}