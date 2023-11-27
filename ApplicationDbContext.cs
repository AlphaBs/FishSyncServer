using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<BucketEntity> Buckets { get; set; } = null!;
    public DbSet<BucketFileEntity> BucketFiles { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<FileChecksumEntity> Checksums { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}