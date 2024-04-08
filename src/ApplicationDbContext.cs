using AlphabetUpdateServer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<ChecksumStorageBucketEntity> Buckets { get; set; } = null!;
    public DbSet<BucketFileEntity> BucketFiles { get; set; } = null!;
    public DbSet<FileChecksumStorageEntity> ChecksumStorages { get; set; } = null!;
    public DbSet<RFilesChecksumStorageEntity> RFilesChecksumStorages { get; set; } = null!;
    public DbSet<ChecksumStorageFileCacheEntity> ChecksumStorageFileCaches { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1:N relationship between ChecksumStorages(1) <-> Buckets(N)
        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .HasOne<FileChecksumStorageEntity>()
            .WithMany()
            .HasForeignKey(e => e.ChecksumStorageId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // 1:N relationship between Buckets(1) <-> BucketFiles(N)
        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .HasMany(e => e.Files)
            .WithOne()
            .HasForeignKey(e => e.BucketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .ComplexProperty(e => e.Limitations);

        modelBuilder.Entity<BucketFileEntity>()
            .HasKey(
            [
                nameof(BucketFileEntity.BucketId),
                nameof(BucketFileEntity.Location)
            ]);
        modelBuilder.Entity<BucketFileEntity>()
            .ComplexProperty(p => p.Metadata);

        modelBuilder.Entity<FileChecksumStorageEntity>()
            .HasDiscriminator(e => e.Type);

        modelBuilder.Entity<RFilesChecksumStorageEntity>()
            .HasBaseType<FileChecksumStorageEntity>();

        modelBuilder.Entity<ChecksumStorageFileCacheEntity>()
            .HasKey(
            [
                nameof(ChecksumStorageFileCacheEntity.StorageId),
                nameof(ChecksumStorageFileCacheEntity.Checksum)
            ]);

        base.OnModelCreating(modelBuilder);
    }
}