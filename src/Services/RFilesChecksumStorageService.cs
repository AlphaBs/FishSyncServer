using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.ChecksumStorages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class RFilesChecksumStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _dbContext;

    public RFilesChecksumStorageService(
        IHttpClientFactory httpClientFactory, 
        ApplicationDbContext dbContext)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
    }

    public bool CanHandle(IChecksumStorage storage)
    {
        return storage is RFilesChecksumStorage;
    }

    public async Task<IEnumerable<IChecksumStorage>> GetAllStorages()
    {
        var storageEntities = await _dbContext.RFilesChecksumStorages.ToListAsync();
        var storages = storageEntities.Select(entityToStorage);
        return storages;
    }

    public async Task<IChecksumStorage?> FindStorageById(string storageId)
    {
        var entity = await _dbContext.RFilesChecksumStorages
            .Where(entity => entity.Id == storageId)
            .FirstOrDefaultAsync();

        if (entity == null)
            return null;

        return entityToStorage(entity);
    }

    public async Task AddStorage(RFilesChecksumStorageEntity storage)
    {
        _dbContext.RFilesChecksumStorages.Add(storage);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> RemoveStorage(string storageId)
    {
        var deleted = await _dbContext.RFilesChecksumStorages
            .Where(entity => entity.Id == storageId)
            .ExecuteDeleteAsync();
        return deleted > 0;
    }

    private IChecksumStorage entityToStorage(RFilesChecksumStorageEntity entity)
    {
        return new RFilesChecksumStorage(
            host: entity.Host, 
            isReadOnly: entity.IsReadyOnly, 
            httpClient: _httpClientFactory.CreateClient());
    }
}