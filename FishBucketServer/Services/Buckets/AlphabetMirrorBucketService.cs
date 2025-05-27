using AlphabetUpdateServer.Entities;
using FishBucket;
using FishBucket.Alphabet;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.Buckets;

public class AlphabetMirrorBucketService : IBucketService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _context;

    public AlphabetMirrorBucketService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public const string AlphabetMirrorType = "alphabet-mirror";
    public string Type => AlphabetMirrorType;
    
    public async Task<IBucket?> Find(string id)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var entity = await _context.AlphabetMirrorBuckets.FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
             return null;
        return new AlphabetMirrorBucket(httpClient, entity.Url, entity.LastUpdated);
    }

    public async Task Create(string id, string url)
    {
        var entity = new AlphabetMirrorBucketEntity
        {
            Id = id,
            LastUpdated = DateTimeOffset.MinValue,
            Limitations = AlphabetMirrorBucket.DefaultLimitations,
            Url = url
        };

        _context.AlphabetMirrorBuckets.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(string id)
    {
        var bucket = new AlphabetMirrorBucketEntity { Id = id };
        _context.AlphabetMirrorBuckets.Attach(bucket);
        _context.AlphabetMirrorBuckets.Remove(bucket);
        await _context.SaveChangesAsync();
    }

    public Task<BucketSyncResult> Sync(string id, IEnumerable<BucketSyncFile> syncFiles)
    {
        throw new BucketLimitationException(BucketLimitationException.ReadonlyBucket);
    }

    public async Task<string> GetOriginUrl(string id)
    {
        var url = await _context.AlphabetMirrorBuckets
            .Where(e => e.Id == id)
            .Select(e => e.Url)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(url))
            throw new KeyNotFoundException(id);

        return url;
    }

    public async Task SetOriginUrl(string id, string url)
    {
        await _context.AlphabetMirrorBuckets
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Url, url));
    }
}