using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.ChecksumStorages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class RFilesChecksumStorageService
{
    private readonly ApplicationDbContext _dbContext;

    public RFilesChecksumStorageService(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ChecksumStorageListItem>> GetAllItems()
    {
        return await _dbContext.RFilesChecksumStorages
            .Select(entity => new ChecksumStorageListItem
            (
                entity.Id, 
                entity.Type, 
                entity.IsReadonly
            ))
            .ToListAsync();
    }

    public async Task<RFilesChecksumStorageEntity?> FindEntityById(string storageId)
    {
        var entity = await _dbContext.RFilesChecksumStorages
            .Where(e => e.Id == storageId)
            .FirstOrDefaultAsync();
        return entity;
    }

    public async Task Create(RFilesChecksumStorageEntity entity)
    {
        _dbContext.RFilesChecksumStorages.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(RFilesChecksumStorageEntity update)
    {
        await _dbContext.RFilesChecksumStorages
            .Where(entity => entity.Id == update.Id)
            .ExecuteUpdateAsync(prop => prop
                .SetProperty(entity => entity.IsReadonly, update.IsReadonly)
                .SetProperty(entity => entity.Host, update.Host)
                .SetProperty(entity => entity.ClientSecret, update.ClientSecret));
    }

    public async Task<bool> Delete(string storageId)
    {
        var deleted = await _dbContext.RFilesChecksumStorages
            .Where(entity => entity.Id == storageId)
            .ExecuteDeleteAsync();
        return deleted > 0;
    }
}