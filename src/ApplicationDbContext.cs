using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<BucketEntity> Buckets { get; set; } = null!;
    public DbSet<BucketFileEntity> BucketFiles { get; set; } = null!;
    public DbSet<FileLocation> Checksums { get; set; } = null!;
    public DbSet<FileChecksumStorageEntity> ChecksumStorages { get; set; } = null!;
    public DbSet<RFilesChecksumStorageEntity> RFilesChecksumStorages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<FileLocation>()
            .HasKey(
                nameof(FileLocation.Location),
                nameof(FileLocation.Checksum));

        modelBuilder
            .Entity<FileChecksumStorageEntity>()
            .HasDiscriminator(e => e.Type);

        modelBuilder
            .Entity<BucketEntity>()
            .HasMany(e => e.Storages)
            .WithOne(e => e.Bucket)
            .HasForeignKey(e => e.BucketId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}