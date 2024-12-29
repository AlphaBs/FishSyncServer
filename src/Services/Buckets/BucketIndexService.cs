using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.Buckets;

public class BucketIndexService
{
    private readonly ApplicationDbContext _context;

    public BucketIndexService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BucketIndex>> GetAllIndexes()
    {
        return await _context.BucketIndexes
            .Select(e => new BucketIndex(e.Id)
            {
                Description = e.Description,
                Searchable = e.Searchable,
            })
            .ToListAsync();
    }

    public async Task<BucketIndex?> FindIndex(string id)
    {
        return await _context.BucketIndexes
            .Where(e => e.Id == id)
            .Select(e => new BucketIndex(e.Id)
            {
                Description = e.Description,
                Searchable = e.Searchable,
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task AddIndex(BucketIndex index)
    {
        var entity = new BucketIndexEntity
        {
            Id = index.Id,
            Description = index.Description,
            Searchable = index.Searchable
        };
        _context.BucketIndexes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveIndex(string id)
    {
        await _context.BucketIndexes
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task UpdateIndexMetadata(BucketIndex index)
    {
        await _context.BucketIndexes
            .Where(e => e.Id == index.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Description, index.Description)
                .SetProperty(e => e.Searchable, index.Searchable));
    }

    public async Task<IEnumerable<string>> GetBucketsFromIndex(string indexId)
    {
        return await _context.BucketIndexes
            .Where(e => e.Id == indexId)
            .Select(e => e.Buckets.Select(bucket => bucket.Id))
            .FirstAsync();
    }
    
    public async Task AddBucketToIndex(string indexId, string bucketId)
    {
        var bucketIndex = new BucketIndexEntity { Id = indexId };
        _context.BucketIndexes.Attach(bucketIndex);
        
        var bucket = new BucketEntity { Id = bucketId };
        _context.Buckets.Attach(bucket);
        
        bucketIndex.Buckets.Add(bucket);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveBucketFromIndex(string indexId, string bucketId)
    {
        var entity = await _context.BucketIndexes
            .Include(e => e.Buckets)
            .FirstOrDefaultAsync(e => e.Id == indexId);

        if (entity == null)
            throw new KeyNotFoundException($"BucketIndex with ID '{indexId}' not found.");

        var bucketToRemove = entity.Buckets.FirstOrDefault(b => b.Id == bucketId);

        if (bucketToRemove == null)
            throw new KeyNotFoundException($"Bucket with ID '{bucketId}' not found in BucketIndex '{indexId}'.");

        entity.Buckets.Remove(bucketToRemove);
        await _context.SaveChangesAsync();
    }
}