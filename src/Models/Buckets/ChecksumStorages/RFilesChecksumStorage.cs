using System.Text.Json;
using AlphabetUpdateServer.Models.Buckets;
using RFiles.NET;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class RFilesChecksumStorage : IFileChecksumStorage
{
    private readonly string _storage;
    private readonly RFilesClient _client;

    public RFilesChecksumStorage(
        string storage, 
        string host, 
        bool isReadOnly,
        HttpClient httpClient)
    {
        Host = host;
        IsReadOnly = isReadOnly;
        _storage = storage;
        _client = new RFilesClient(host, httpClient, JsonSerializerOptions.Default);
    }

    public string Host { get; }
    public bool IsReadOnly { get; private set; }

    public BucketSyncAction CreateSyncAction(BucketSyncFile syncFile)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("ReadOnly storage");
        }

        return new BucketSyncAction
        {
            ActionType = BucketSyncActionTypes.Http,
            File = syncFile
        };
    }

    public async IAsyncEnumerable<FileLocation> GetAllFiles()
    {
        var objects = await _client.GetAllObjects();
        var locations = objects.Select(toFileLocation);
        foreach (var location in locations)
        {
            yield return location;
        }
    }

    public async IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums)
    {
        var objects = await _client.Query(checksums);
        var locations = objects.Select(toFileLocation);
        foreach (var location in locations)
        {
            yield return location;
        }
    }

    private FileLocation toFileLocation(RFilesObjectMetadata metadata) =>
        new FileLocation(
            metadata: new BucketFileMetadata(
                Size: metadata.Size,
                LastUpdated: metadata.Uploaded,
                Checksum: metadata.Hash),
            storage: _storage,  
            location: _client.GetObjectUri(metadata.Hash).ToString());
}