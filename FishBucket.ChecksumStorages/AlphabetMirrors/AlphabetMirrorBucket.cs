using System.Text.Json;

namespace FishBucket.Alphabet;

public class AlphabetMirrorBucket : IBucket
{
    public static readonly BucketLimitations DefaultLimitations = new()
    {
        IsReadOnly = true,
        MaxFileSize = long.MaxValue,
        MaxNumberOfFiles = long.MaxValue,
        MaxBucketSize = long.MaxValue,
        ExpiredAt = DateTimeOffset.MaxValue,
        MonthlyMaxSyncCount = int.MaxValue
    };
    
    private readonly HttpClient _httpClient;
    
    public AlphabetMirrorBucket(HttpClient httpClient, string origin, DateTimeOffset lastUpdated)
    {
        _httpClient = httpClient;
        OriginUrl = origin;
        LastUpdated = lastUpdated;
    }
    
    public string OriginUrl { get; }
    public DateTimeOffset LastUpdated { get; }

    public BucketLimitations Limitations => DefaultLimitations;
    
    public async ValueTask<IEnumerable<BucketFile>> GetFiles()
    {
        var metadata = await getMetadata();
        var files = metadata.Files?.Files ?? [];
        return convert(files, metadata.Files?.LastUpdate ?? DateTimeOffset.MinValue);

        static IEnumerable<BucketFile> convert(IEnumerable<UpdateFile> files, DateTimeOffset lastUpdated)
        {
            foreach (var file in files)
            {
                if (string.IsNullOrEmpty(file.Path))
                    continue;
                
                yield return new BucketFile(
                    file.Path, 
                    file.Url, 
                    new FileMetadata(
                        file.Size, 
                        lastUpdated, 
                        file.Hash ?? ""));
            }
        }
    }

    private async Task<LauncherMetadata> getMetadata()
    {
        using var res = await _httpClient.GetAsync(OriginUrl, HttpCompletionOption.ResponseHeadersRead);
        res.EnsureSuccessStatusCode();
        await using var stream = await res.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<LauncherMetadata>(stream) ?? throw new FormatException("Null response");
    }

    public ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> syncFiles)
    {
        throw new BucketLimitationException(BucketLimitationException.ReadonlyBucket);
    }
}