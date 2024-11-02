using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.ChecksumStorages;

public class ObjectChecksumStorageService
{
    private readonly ApplicationDbContext _context;

    public ObjectChecksumStorageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChecksumStorageListItem>> GetAllItems()
    {
        return await _context.ObjectChecksumStorages
            .Select(entity => new ChecksumStorageListItem
            (
                entity.Id,
                entity.Type,
                entity.IsReadonly
            ))
            .ToListAsync();
    }

    public async Task<ObjectChecksumStorageEntity?> FindEntityById(string id)
    {
        return await _context.ObjectChecksumStorages
            .FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task Create(ObjectChecksumStorageEntity entity)
    {
        _context.ObjectChecksumStorages.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ObjectChecksumStorageEntity update)
    {
        await _context.ObjectChecksumStorages
            .Where(entity => entity.Id == update.Id)
            .ExecuteUpdateAsync(prop => prop
                .SetProperty(entity => entity.IsReadonly, update.IsReadonly)
                .SetProperty(entity => entity.AccessKey, update.AccessKey)
                .SetProperty(entity => entity.PublicEndpoint, update.PublicEndpoint)
                .SetProperty(entity => entity.BucketName, update.BucketName)
                .SetProperty(entity => entity.SecretKey, update.SecretKey)
                .SetProperty(entity => entity.Prefix, update.Prefix));
    }

    public async Task<bool> Delete(string id)
    {
        var deleted = await _context.ObjectChecksumStorages
            .Where(entity => entity.Id == id)
            .ExecuteDeleteAsync();
        return deleted > 0;
    }
}