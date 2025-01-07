using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services.Buckets;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.Users;

public class BucketOwnerService
{
    private readonly ApplicationDbContext _context;
    
    public BucketOwnerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CheckOwnershipByUsername(string bucketId, string username)
    {
        return await _context.Buckets
            .Where(bucket => bucket.Id == bucketId)
            .SelectMany(bucket => bucket.Owners.Select(user => user.Username))
            .AnyAsync(e => e == username);
    }
    
    public IAsyncEnumerable<string> GetOwners(string bucketId)
    {
        return _context.Buckets
            .Where(bucket => bucket.Id == bucketId)
            .SelectMany(bucket => bucket.Owners)
            .Select(user => user.Username)
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<string> GetBuckets(string username)
    {
        return _context.Users
            .Where(user => user.Username == username)
            .SelectMany(user => user.Buckets.Select(bucket => bucket.Id))
            .AsAsyncEnumerable();
    }
    
    public async Task AddOwner(string bucketId, string username)
    {
        var bucket = new BucketEntity { Id = bucketId };
        _context.Buckets.Attach(bucket);
        
        var user = new UserEntity { Username = username };
        _context.Users.Attach(user);
        
        bucket.Owners.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveOwner(string bucketId, string username)
    {
        var bucket = await _context.Buckets
            .IgnoreAutoIncludes()
            .Include(bucket => bucket.Owners)
            .FirstOrDefaultAsync(bucket => bucket.Id == bucketId);
        if (bucket == null)
            throw new KeyNotFoundException("Bucket not found");

        var user = bucket.Owners.FirstOrDefault(user => user.Username == username);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        bucket.Owners.Remove(user);
        await _context.SaveChangesAsync();
    }
}