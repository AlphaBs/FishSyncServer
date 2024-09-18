using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<ConfigEntity> Configs { get; set; } = null!;
    public DbSet<ChecksumStorageBucketEntity> Buckets { get; set; } = null!;
    public DbSet<ChecksumStorageBucketFileEntity> ChecksumStorageBucketFiles { get; set; } = null!;
    public DbSet<ChecksumStorageEntity> ChecksumStorages { get; set; } = null!;
    public DbSet<RFilesChecksumStorageEntity> RFilesChecksumStorages { get; set; } = null!;
    public DbSet<ObjectChecksumStorageEntity> ObjectChecksumStorages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .HasMany(e => e.Owners)
            .WithMany(e => e.Buckets)
            .UsingEntity<BucketOwnerUserEntity>();
        
        // 1:N relationship between ChecksumStorages(1) <-> Buckets(N)
        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .HasOne<ChecksumStorageEntity>()
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

        modelBuilder.Entity<ChecksumStorageBucketFileEntity>()
            .HasKey(
            [
                nameof(ChecksumStorageBucketFileEntity.BucketId),
                nameof(ChecksumStorageBucketFileEntity.Path)
            ]);
        modelBuilder.Entity<ChecksumStorageBucketFileEntity>()
            .ComplexProperty(p => p.Metadata);

        modelBuilder.Entity<ChecksumStorageEntity>()
            .HasDiscriminator(e => e.Type)
            .HasValue<ChecksumStorageEntity>("base")
            .HasValue<RFilesChecksumStorageEntity>(RFilesChecksumStorageEntity.RFilesType)
            .HasValue<ObjectChecksumStorageEntity>(ObjectChecksumStorageEntity.ObjectType);

        modelBuilder.Entity<RFilesChecksumStorageEntity>()
            .HasBaseType<ChecksumStorageEntity>();

        modelBuilder.Entity<RFilesChecksumStorageEntity>()
            .HasBaseType<ChecksumStorageEntity>();

        base.OnModelCreating(modelBuilder);
    }
}