using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Repositories;

public class FileChecksumStorageDbRepository : IFileChecksumStorageRepository
{
    private readonly ApplicationDbContext _context;

    public FileChecksumStorageDbRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask Add(FileChecksumStorageEntity storage)
    {
        await _context.ChecksumStorages.AddAsync(storage);
        await _context.SaveChangesAsync();
    }

    public async ValueTask<FileChecksumStorageEntity?> FindStorage(string bucketId, string id)
    {
        return await _context.ChecksumStorages
            .FirstAsync(storage => 
                storage.BucketId == bucketId && 
                storage.Id == id);
    }

    public async ValueTask<IEnumerable<FileChecksumStorageEntity>> GetStorages(string bucketId)
    {
        return await _context.ChecksumStorages
            .Where(storage => storage.BucketId == bucketId)
            .ToListAsync();
    }

    public async ValueTask Remove(string bucketId, string id)
    {
        await _context.ChecksumStorages
            .Where(storage => storage.Id == id)
            .ExecuteDeleteAsync();
    }
}