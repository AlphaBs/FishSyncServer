using System.Net.Http.Headers;
using System.Text.Json;

namespace FishBucket.ApiClient;

public class FishApiClient : IFishApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _host;

    public FishApiClient(string host, HttpClient httpClient) => 
        (_host, _httpClient) = 
        (host, httpClient);

    public string? ApiKey { get; set; }

    public async Task<IReadOnlyList<string>> ListBuckets(CancellationToken cancellationToken = default)
    {
        using var reqMessage = new HttpRequestMessage();
        reqMessage.RequestUri = new Uri(_host + "/buckets");
        reqMessage.Method = HttpMethod.Get;
        await using var resStream = await request(reqMessage, cancellationToken);
        using var json = await JsonDocument.ParseAsync(resStream, cancellationToken: cancellationToken);
        
        try
        {
            return json.RootElement
                .GetProperty("buckets")
                .EnumerateArray()
                .Select(elem => elem.GetString())
                .Where(elem => !string.IsNullOrEmpty(elem))
                .ToList()!;
        }
        catch (KeyNotFoundException)
        {
            throw new FormatException();
        }
        catch (InvalidOperationException)
        {
            throw new FormatException();
        }
    }

    public async Task<FishBucketResponse> GetBucket(string id, CancellationToken cancellationToken = default)
    {
        using var reqMessage = new HttpRequestMessage();
        reqMessage.RequestUri = new Uri($"{_host}/buckets/common/{id}");
        reqMessage.Method = HttpMethod.Get;
        await using var resStream = await request(reqMessage, cancellationToken);
        var json = await JsonSerializer.DeserializeAsync<FishBucketResponse>(resStream, cancellationToken: cancellationToken);
        return json ?? throw new FormatException();
    }
    
    public async Task<FishBucketFilesResponse> GetBucketFilesRecursively(string id, CancellationToken cancellationToken = default)
    {
        return await FishBucketDependencyResolver.Resolve(this, id, 8, cancellationToken);
    }

    public async Task<FishBucketFilesResponse> GetBucketFiles(string id, CancellationToken cancellationToken = default)
    {
        using var reqMessage = new HttpRequestMessage();
        reqMessage.RequestUri = new Uri($"{_host}/buckets/common/{id}/files");
        reqMessage.Method = HttpMethod.Get;
        await using var resStream = await request(reqMessage, cancellationToken);
        var json = await JsonSerializer.DeserializeAsync<FishBucketFilesResponse>(resStream, cancellationToken: cancellationToken);
        return json ?? throw new FormatException();
    }

    public async Task<BucketSyncResult> Sync(
        string id, 
        IEnumerable<BucketSyncFile> files, 
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(new { files });
        using var reqContent = new StringContent(json);
        reqContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var reqMessage = new HttpRequestMessage();
        reqMessage.RequestUri = new Uri($"{_host}/buckets/common/{id}/sync");
        reqMessage.Method = HttpMethod.Post;
        reqMessage.Content = reqContent;
        reqMessage.Headers.Add("Authorization", $"Bearer {ApiKey}");
        await using var resStream = await request(reqMessage, cancellationToken);
        return await JsonSerializer.DeserializeAsync<BucketSyncResult>(resStream, cancellationToken: cancellationToken) ??
            throw new FormatException();
    }

    public async Task<string> Login(
        string username, 
        string password, 
        CancellationToken cancellationToken = default, 
        bool setApiKey = true)
    {
        var reqJson = JsonSerializer.Serialize(new { username, password });
        using var reqContent = new StringContent(reqJson);
        reqContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var reqMessage = new HttpRequestMessage();
        reqMessage.RequestUri = new Uri($"{_host}/auth/login");
        reqMessage.Method = HttpMethod.Post;
        reqMessage.Content = reqContent;
        await using var resStream = await request(reqMessage, cancellationToken);
        using var resJson = await JsonDocument.ParseAsync(resStream, cancellationToken: cancellationToken);
        var token = resJson.RootElement.GetProperty("token").GetString() ?? "";
        if (setApiKey)
            ApiKey = token;
        return token;
    }

    private async Task<Stream> request(HttpRequestMessage message, CancellationToken cancellationToken)
    {
        var res = await _httpClient.SendAsync(message, cancellationToken);
        var resStream = await res.Content.ReadAsStreamAsync();
        if (res.IsSuccessStatusCode)
        {
            return resStream;
        }
        else
        {
            var exception = await parseErrorResponse(resStream, (int)res.StatusCode);
            await resStream.DisposeAsync();
            throw exception;
        }
    }

    private async Task<Exception> parseErrorResponse(Stream stream, int status)
    {
        try
        {
            var json = await JsonSerializer.DeserializeAsync<ProblemDetails>(stream) 
                ?? throw new FormatException();
            return new FishApiException(json);
        }
        catch (FormatException)
        {
            using var sr = new StreamReader(stream);
            var message = await sr.ReadToEndAsync();
            return new FishApiException(message, status);
        }
    }
}