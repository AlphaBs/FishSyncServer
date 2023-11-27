using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Repositories;

public class FileChecksumDbRepository : IFileChecksumRepository
{
    private readonly ApplicationDbContext _context;

    public FileChecksumDbRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IEnumerable<FileChecksumEntity>> Find(string checksum)
    {
        return await _context.Checksums
            .Where(file => file.Checksum == checksum)
            .ToListAsync();
    }

    public async ValueTask<FileChecksumEntity?> Find(string checksum, string repository)
    {
        return await _context.Checksums.SingleOrDefaultAsync(file => 
            file.Checksum == checksum && file.Repository == repository);
    }

    public async ValueTask<FileChecksumEntity?> FindBest(string checksum)
    {
        return await _context.Checksums.SingleOrDefaultAsync(file =>
            file.Checksum == checksum);
    }

    public async ValueTask<FileChecksumEntity[]> BulkFind(IEnumerable<string> checksums)
    {
        var checksumsArr = checksums.ToArray();
        var files = await _context.Checksums
            .Where(file => checksumsArr.Contains(file.Checksum))
            .ToListAsync();
        return files.ToArray();
    }

    public async ValueTask<IEnumerable<FileChecksumEntity>> GetAllFiles()
    {
        return await _context.Checksums.ToListAsync();
    }

    public async ValueTask<IEnumerable<FileChecksumEntity>> GetAllFilesFromRepository(string repository)
    {
        return await _context.Checksums
            .Where(file => file.Repository == repository)
            .ToListAsync();
    }

    public async ValueTask Add(FileChecksumEntity entity)
    {
        await _context.Checksums.AddAsync(entity);
        await _context.SaveChangesAsync();
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
            .Where(file => file.Checksum == checksum && file.Repository == repository)
            .ExecuteDeleteAsync();
    }
}