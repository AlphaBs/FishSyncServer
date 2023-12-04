using AlphabetUpdateServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Repositories;

public class CachedFileChecksumDbRepository : ICachedFileChecksumRepository
{
    private readonly ApplicationDbContext _context;

    public CachedFileChecksumDbRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask Remove(string checksum)
    {
        await _context.Checksums
            .Where(file => file.Checksum == checksum)
            .ExecuteDeleteAsync();
    }

    public async ValueTask Remove(string checksum, string repository)
    {
        await _context.Checksums
            .Where(file => file.Checksum == checksum && file.Storage == repository)
            .ExecuteDeleteAsync();
    }

    public IAsyncEnumerable<FileLocation> GetAllFiles()
    {
        return _context.Checksums.AsAsyncEnumerable();
    }

    public IAsyncEnumerable<FileLocation> Query(IEnumerable<string> checksums)
    {
        var checksumsArr = checksums.ToArray();
        return _context.Checksums
            .Where(file => checksumsArr.Contains(file.Checksum))
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<FileLocation> Find(string checksum)
    {
        return _context.Checksums
            .Where(file => file.Checksum == checksum)
            .AsAsyncEnumerable();
    }

    public async ValueTask<FileLocation?> Find(string checksum, string storage)
    {
        return await _context.Checksums.SingleOrDefaultAsync(file => 
            file.Checksum == checksum && file.Storage == storage);
    }

    public async ValueTask<FileLocation?> FindBest(string checksum)
    {
        return await _context.Checksums.SingleOrDefaultAsync(file =>
            file.Checksum == checksum);
    }

    public async ValueTask Add(FileLocation entity)
    {
        await _context.Checksums.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async ValueTask Add(IEnumerable<FileLocation> entities)
    {
        foreach (var entity in entities)
        {
            await _context.Checksums.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
    }
}