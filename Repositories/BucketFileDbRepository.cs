using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Repositories;

public class BucketFileDbRepository : IBucketFileRepository
{
    private readonly ApplicationDbContext _context;

    public BucketFileDbRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IEnumerable<BucketFileEntity>> GetFiles(string bucketId)
    {
        return await _context.BucketFiles
            .Where(file => file.BucketId == bucketId)
            .ToListAsync();
    }

    public async ValueTask UpdateFiles(string bucketId, IEnumerable<BucketFileEntity> files)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        await _context.BucketFiles.Where(file => file.BucketId == bucketId).ExecuteDeleteAsync();
        await _context.BucketFiles.AddRangeAsync(files);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}