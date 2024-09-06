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

public class ObjectChecksumStorage : IChecksumStorage
{
    private readonly ObjectStorageConfig _storage;

    public ObjectChecksumStorage(ObjectStorageConfig config)
    {
        _storage = config;
    }

    public bool IsReadOnly => false;

    public async IAsyncEnumerable<ChecksumStorageFile> GetAllFiles()
    {
        using var client = createClient();
        var pages = client.Paginators.ListObjectsV2(new ListObjectsV2Request
        {
            BucketName = _storage.BucketName,
            Prefix = _storage.Prefix
        });

        await foreach (var response in pages.Responses)
        {
            foreach (var entry in response.S3Objects)
            {
                yield return new ChecksumStorageFile
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
            }
        }
    }

    public async IAsyncEnumerable<ChecksumStorageFile> Query(IEnumerable<string> checksums)
    {
        var tasks = checksums.Select(query).ToList();
        await Task.WhenAll(tasks);
        foreach (var task in tasks)
        {
            var file = await task;
            if (file != null)
                yield return file;
        }

        async Task<ChecksumStorageFile?> query(string checksum)
        {
            var metadata = await getMetadata(checksum);
            if (metadata == null)
                return null;
            else
                return objectToFile(checksum, metadata);
        }
    }

    public async Task<ChecksumStorageSyncResult> Sync(IEnumerable<string> checksums)
    {
        using var client = createClient();
        var successFiles = new List<ChecksumStorageFile>();
        var requiredActions = new List<ChecksumStorageSyncAction>();

        var tasks = checksums.Select(sync).ToList();
        await Task.WhenAll(tasks);
        foreach (var task in tasks)
        {
            await task;
        }

        return new ChecksumStorageSyncResult(successFiles, requiredActions);

        async Task sync(string checksum)
        {
            var metadata = await getMetadata(checksum);
            if (metadata == null)
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _storage.BucketName,
                    Key = getKeyFromChecksum(checksum),
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                };
                request.Headers.ContentMD5 = Convert.ToBase64String(Convert.FromHexString(checksum));

                var presignedUrl = client.GetPreSignedURL(request);
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
                requiredActions.Add(action);
            }
            else
            {
                var file = objectToFile(checksum, metadata);
                successFiles.Add(file);
            }
        }
    }

    private async Task<GetObjectMetadataResponse?> getMetadata(string checksum)
    {
        using var client = createClient();
        try
        {
            var key = getKeyFromChecksum(checksum);
            var response = await client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                Key = key,
                BucketName = _storage.BucketName
            });
            return response;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
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

    private IAmazonS3 createClient()
    {
        AWSConfigsS3.UseSignatureVersion4 = true;
        return new AmazonS3Client(_storage.AccessKey, _storage.SecretKey, new AmazonS3Config
        {
            ServiceURL = _storage.ServiceEndpoint
        });
    }
}
