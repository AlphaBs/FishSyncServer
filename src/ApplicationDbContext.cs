using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<BucketEntity> Buckets { get; set; } = null!;
    public DbSet<BucketFile> BucketFiles { get; set; } = null!;
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

        modelBuilder
            .Entity<BucketFile>()
            .HasKey(
                nameof(BucketFile.BucketId),
                nameof(BucketFile.Path));

        base.OnModelCreating(modelBuilder);
    }
}