using System.Text.Json;
using AlphabetUpdateServer.Models.Buckets;
using RFiles.NET;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class RFilesChecksumStorage : IFileChecksumStorage
{
    private readonly string _storage;
    private readonly RFilesClient _client;

    public RFilesChecksumStorage(string storage, string host, HttpClient httpClient)
    {
        Host = host;
        _storage = storage;
        _client = new RFilesClient(host, httpClient, JsonSerializerOptions.Default);
    }

    public string Host { get; }
    public bool IsReadOnly => true;

    public BucketSyncAction CreateSyncAction(BucketSyncFile syncFile)
    {
        return new BucketSyncAction(syncFile)
        {
            ActionType = BucketSyncActionTypes.Http,

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

    private FileLocation toFileLocation(RFilesObjectMetadata metadata)
    {
        return new FileLocation(
            checksum: metadata.Hash, 
            storage: _storage, 
            size: metadata.Size, 
            location: _client.GetObjectUri(metadata.Hash).ToString());
    }
}