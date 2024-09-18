using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.Buckets;

public class BucketOwnerService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    
    public BucketOwnerService(ApplicationDbContext context, UserManager<User> userManager)
    {
        _dbContext = context;
        _userManager = userManager;
    }
    
    public async Task<IEnumerable<string>> GetOwners(string bucketId)
    {
        var usernames = await _dbContext.Buckets
            .Where(bucket => bucket.Id == bucketId)
            .SelectMany(bucket => bucket.Owners)
            .Select(user => user.UserName)
            .ToListAsync();

        return usernames.Where(username => !string.IsNullOrEmpty(username))!;
    }

    public async Task AddOwner(string bucketId, string username)
    {
        var bucket = new ChecksumStorageBucketEntity { Id = bucketId };
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        _dbContext.Buckets.Attach(bucket);
        
        bucket.OwnerUserEntities.Add(new BucketOwnerUserEntity
        {
            ChecksumStorageBucketEntityId = bucketId,
            UserId = user.Id
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveOwner(string bucketId, string username)
    {
        var bucket = await _dbContext.Buckets
            .IgnoreAutoIncludes()
            .Include(bucket => bucket.OwnerUserEntities)
            .FirstOrDefaultAsync(bucket => bucket.Id == bucketId);
        if (bucket == null)
            throw new KeyNotFoundException("Bucket not found");
        
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            throw new KeyNotFoundException("User not found");
        
        bucket.OwnerUserEntities.Remove(bucket.OwnerUserEntities.First(entity => entity.UserId == user.Id));
        await _dbContext.SaveChangesAsync();
    }
}