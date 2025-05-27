using AlphabetUpdateServer.Entities;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<ConfigEntity> Configs { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    
    public DbSet<BucketEntity> Buckets { get; set; } = null!;
    public DbSet<AlphabetMirrorBucketEntity> AlphabetMirrorBuckets { get; set; } = null!;
    public DbSet<ChecksumStorageBucketEntity> ChecksumStorageBuckets { get; set; } = null!;
    public DbSet<ChecksumStorageBucketFileEntity> ChecksumStorageBucketFiles { get; set; } = null!;
    public DbSet<BucketSyncEventEntity> BucketSyncEvents { get; set; } = null!;
    public DbSet<BucketIndexEntity> BucketIndexes { get; set; } = null!;
    
    public DbSet<ChecksumStorageEntity> ChecksumStorages { get; set; } = null!;
    public DbSet<RFilesChecksumStorageEntity> RFilesChecksumStorages { get; set; } = null!;
    public DbSet<ObjectChecksumStorageEntity> ObjectChecksumStorages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BucketEntity>()
            .UseTptMappingStrategy();
        
        modelBuilder.Entity<BucketEntity>()
            .ComplexProperty(e => e.Limitations);
        
        // N:M relationship between Users <-> Buckets
        modelBuilder.Entity<UserEntity>()
            .HasMany(e => e.Buckets)
            .WithMany(e => e.Owners);

        // N:M relationship between Users <-> Buckets
        modelBuilder.Entity<BucketEntity>()
            .HasMany(e => e.Owners)
            .WithMany(e => e.Buckets);

        // N:M relationship between Buckets <-> Buckets
        modelBuilder.Entity<BucketEntity>()
            .HasMany(e => e.Dependencies)
            .WithMany();
        
        // 1:N relationship between ChecksumStorages(1) <-> Buckets(N)
        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .HasOne<ChecksumStorageEntity>()
            .WithMany()
            .HasForeignKey(e => e.ChecksumStorageId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:N relationship between ChecksumStorageBucket(1) <-> BucketFiles(N)
        modelBuilder.Entity<ChecksumStorageBucketEntity>()
            .HasMany(e => e.Files)
            .WithOne()
            .HasForeignKey(e => e.BucketId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChecksumStorageBucketFileEntity
        modelBuilder.Entity<ChecksumStorageBucketFileEntity>()
            .HasKey(
            [
                nameof(ChecksumStorageBucketFileEntity.BucketId),
                nameof(ChecksumStorageBucketFileEntity.Path)
            ]);
        modelBuilder.Entity<ChecksumStorageBucketFileEntity>()
            .ComplexProperty(p => p.Metadata);

        // ChecksumStorageEntity
        modelBuilder.Entity<ChecksumStorageEntity>()
            .HasDiscriminator(e => e.Type)
            .HasValue<ChecksumStorageEntity>("base")
            .HasValue<RFilesChecksumStorageEntity>(RFilesChecksumStorageEntity.RFilesType)
            .HasValue<ObjectChecksumStorageEntity>(ObjectChecksumStorageEntity.ObjectType);

        modelBuilder.Entity<RFilesChecksumStorageEntity>()
            .HasBaseType<ChecksumStorageEntity>();

        // BucketSyncEventEntity
        modelBuilder.Entity<BucketSyncEventEntity>()
            .HasOne<BucketEntity>()
            .WithMany()
            .HasForeignKey(e => e.BucketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BucketSyncEventEntity>()
            .HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // BucketIndexEntity
        modelBuilder.Entity<BucketIndexEntity>()
            .HasMany(e => e.Buckets)
            .WithMany();
        
        base.OnModelCreating(modelBuilder);
    }
}