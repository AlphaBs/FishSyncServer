using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public record ObjectStorageConfig
(
    string AccessKey,
    string SecretKey,
    string BucketName,
    string Prefix,
    string ServiceEndpoint,
    string PublicEndpoint
);

public class ObjectChecksumStorage : IChecksumStorage, IDisposable
{
    private readonly ObjectStorageConfig _storage;
    private readonly IAmazonS3 _client;

    public ObjectChecksumStorage(ObjectStorageConfig config)
    {
        _storage = config;
        
        AWSConfigsS3.UseSignatureVersion4 = true;
        _client = new AmazonS3Client(_storage.AccessKey, _storage.SecretKey, new AmazonS3Config
        {
            ServiceURL = _storage.ServiceEndpoint
        });
    }

    public bool IsReadOnly => false;

    public async Task<IEnumerable<ChecksumStorageFile>> GetAllFiles()
    {
        var pages = _client.Paginators.ListObjectsV2(new ListObjectsV2Request
        {
            BucketName = _storage.BucketName,
            Prefix = _storage.Prefix
        });

        var files = new List<ChecksumStorageFile>();
        await foreach (var response in pages.Responses)
        {
            foreach (var entry in response.S3Objects)
            {
                var file = new ChecksumStorageFile
                (
                    Checksum: getChecksumFromKey(entry.Key),
                    Location: getUrlFromKey(entry.Key),
                    new FileMetadata
                    (
                        entry.Size,
                        entry.LastModified,
                        getChecksumFromKey(entry.Key)
                    )
                );
                files.Add(file);
            }
        }

        return files;
    }

    public async Task<ChecksumStorageQueryResult> Query(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        var tasks = checksumSet.Select(query).ToList();
        await Task.WhenAll(tasks);
            
        var foundFiles = new List<ChecksumStorageFile>();
        var notFoundChecksums = new List<string>();
        foreach (var task in tasks)
        {
            var (result, obj) = await task;
            if (result)
                foundFiles.Add((ChecksumStorageFile)obj);
            else
                notFoundChecksums.Add((string)obj);
        }

        return new ChecksumStorageQueryResult(foundFiles, notFoundChecksums);

        async Task<(bool, object)> query(string checksum)
        {
            try
            {
                var metadata = await requestFileMetadata(checksum);
                return (true, objectToFile(checksum, metadata));
            }
            catch (FileNotFoundException)
            {
                return (false, checksum);
            }
        }
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        var checksumSet = checksums.ToHashSet();
        var successFiles = new List<ChecksumStorageFile>();
        var requiredActions = new List<ChecksumStorageSyncAction>();

        var tasks = checksumSet.Select(sync).ToList();
        await Task.WhenAll(tasks);
        foreach (var task in tasks)
        {
            var (result, obj) = await task;
            if (result)
                successFiles.Add((ChecksumStorageFile)obj);
            else
                requiredActions.Add((ChecksumStorageSyncAction)obj);
        }

        return new ChecksumStorageSyncResult(successFiles, requiredActions);

        async Task<(bool, object)> sync(string checksum)
        {
            try
            {
                var metadata = await requestFileMetadata(checksum);
                var file = objectToFile(checksum, metadata);
                return (true, file);
            }
            catch (FileNotFoundException)
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _storage.BucketName,
                    Key = getKeyFromChecksum(checksum),
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    Headers =
                    {
                        ContentMD5 = Convert.ToBase64String(Convert.FromHexString(checksum))
                    }
                };

                var presignedUrl = _client.GetPreSignedURL(request);
                var action = new ChecksumStorageSyncAction
                (
                    Checksum: checksum,
                    Action: SyncActionFactory.CreateHttpRequest
                    (
                        method: "PUT", 
                        url: presignedUrl, 
                        headers: new Dictionary<string, string>
                        {
                            ["Content-MD5"] = request.Headers.ContentMD5
                        }
                    )
                );
                return (false, action);
            }
        }
    }
    
    private async Task<GetObjectMetadataResponse> requestFileMetadata(string checksum)
    {
        try
        {
            var key = getKeyFromChecksum(checksum);
            var response = await _client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                Key = key,
                BucketName = _storage.BucketName
            });
            return response;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException("Cannot find a object", checksum, e);
        }
    }

    private ChecksumStorageFile objectToFile(string checksum, GetObjectMetadataResponse response)
    {
        var key = getKeyFromChecksum(checksum);
        return new ChecksumStorageFile
        (
            Checksum: checksum,
            Location: getUrlFromKey(key),
            new FileMetadata
            (
                Size: response.ContentLength,
                LastUpdated: response.LastModified,
                Checksum: checksum
            )
        );
    }

    private string getChecksumFromKey(string key)
    {
        return key.Substring(_storage.Prefix.Length);
    }

    private string getKeyFromChecksum(string checksum)
    {
        return _storage.Prefix + checksum;
    }

    private string getUrlFromKey(string key)
    {
        return _storage.PublicEndpoint + "/" + key;
    }

    public void Dispose() => _client.Dispose();
}
