using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Tests.Buckets;

public class ChecksumStorageBucketTests
{
    private readonly IChecksumStorage TestChecksumStorage;

    public ChecksumStorageBucketTests()
    {
        var storage = new InMemoryChecksumStorage();
        storage.AddRange(
        [
            new ChecksumStorageFile
            (
                Checksum: "file1_checksum",
                Location: "file1_location",
                Metadata: new Models.FileMetadata
                (
                    Size: 1001,
                    LastUpdated: DateTimeOffset.MinValue,
                    Checksum: "file1_checksum"
                )
            ),
            new ChecksumStorageFile
            (
                Checksum: "file2_checksum",
                Location: "file2_location",
                Metadata: new Models.FileMetadata
                (
                    Size: 1002,
                    LastUpdated: DateTimeOffset.MinValue,
                    Checksum: "file2_checksum"
                )
            )
        ]);
        TestChecksumStorage = storage;
    }

    private ChecksumStorageBucket CreateTestBucket(BucketLimitations limitations)
    {
        return new ChecksumStorageBucket(limitations, TestChecksumStorage);
    }

    [Fact]
    public async Task successful_sync_single_existent_file()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            }
        ]);

        // Then
        Assert.True(syncResult.IsSuccess);
        Assert.Empty(syncResult.RequiredActions);

        var file = Assert.Single(await bucket.GetFiles());
        Assert.Equal("file1.zip", file.Path);
        Assert.Equal("file1_location", file.Location);
        Assert.Equal("file1_checksum", file.Metadata.Checksum);
    }

    [Fact]
    public async Task successful_sync_same_checksum_but_different_path()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            },
            new BucketSyncFile
            {
                Path = "file2.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            }
        ]);

        // Then
        Assert.True(syncResult.IsSuccess);
        Assert.Empty(syncResult.RequiredActions);

        var actualFiles = await bucket.GetFiles();
        Assert.Equal(["file1.zip", "file2.zip"], actualFiles.Select(file => file.Path));
        Assert.Equal(["file1_checksum", "file1_checksum"], actualFiles.Select(file => file.Metadata.Checksum));
        Assert.Equal([1001, 1001], actualFiles.Select(file => file.Metadata.Size));
    }

    [Fact]
    public async Task create_sync_action_for_single_nonexistent_file()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file?.zip",
                Size = 1001,
                Checksum = "file?_checksum"
            }
        ]);

        // Then
        Assert.False(syncResult.IsSuccess);
        Assert.Empty(await bucket.GetFiles());

        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("file?.zip", action.Path);
    }

    [Fact]
    public async Task create_sync_action_only_for_nonexistent_file()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file?.zip",
                Size = 1234,
                Checksum = "file?_checksum"
            },
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            }
        ]);

        // Then
        Assert.False(syncResult.IsSuccess);
        Assert.Empty(await bucket.GetFiles());

        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("file?.zip", action.Path);
    }

    [Fact]
    public async Task fail_sync_to_readonly_bucket()
    {
        // Given
        var bucket = CreateTestBucket(new BucketLimitations
        {
            IsReadOnly = true,
            ExpiredAt = DateTimeOffset.MaxValue,
            MaxBucketSize = 1024,
            MaxFileSize = 1024,
            MaxNumberOfFiles = 1024
        });

        // When
        var exception = await Assert.ThrowsAsync<BucketLimitationException>(async () =>
        {
            await bucket.Sync(
            [
                new BucketSyncFile
                {
                    Path = "file1.zip",
                    Size = 1001,
                    Checksum = "file1_checksum"
                }
            ]);
        });

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.Equal("readonly_bucket", exception.Reason);
    }

    [Fact]
    public async Task fail_sync_to_expired_bucket()
    {
        // Given
        var bucket = CreateTestBucket(new BucketLimitations
        {
            IsReadOnly = false,
            ExpiredAt = DateTimeOffset.MinValue,
            MaxBucketSize = 1024,
            MaxFileSize = 1024,
            MaxNumberOfFiles = 1024
        });

        // When
        var exception = await Assert.ThrowsAsync<BucketLimitationException>(async () =>
        {
            await bucket.Sync(
            [
                new BucketSyncFile
                {
                    Path = "file1.zip",
                    Size = 1001,
                    Checksum = "file1_checksum"
                }
            ]);
        });

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.Equal("expired_bucket", exception.Reason);
    }

    [Fact]
    public async Task fail_sync_when_exceeding_max_bucket_size()
    {
        // Given
        var bucket = CreateTestBucket(new BucketLimitations
        {
            IsReadOnly = false,
            ExpiredAt = DateTimeOffset.MaxValue,
            MaxBucketSize = 1024,
            MaxFileSize = 1024,
            MaxNumberOfFiles = 1024
        });

        // When
        var exception = await Assert.ThrowsAsync<BucketLimitationException>(async () =>
        {
            await bucket.Sync(
            [
                new BucketSyncFile
                {
                    Path = "file1.zip",
                    Size = 1001,
                    Checksum = "file1_checksum"
                },
                new BucketSyncFile
                {
                    Path = "file2.zip",
                    Size = 1002,
                    Checksum = "file2_checksum"
                }
            ]);
        });

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.Equal("exceed_max_bucket_size", exception.Reason);
    }

    [Fact]
    public async Task fail_sync_when_exceeding_max_number_of_files()
    {
        // Given
        var bucket = CreateTestBucket(new BucketLimitations
        {
            IsReadOnly = false,
            ExpiredAt = DateTimeOffset.MaxValue,
            MaxBucketSize = 2048,
            MaxFileSize = 1024,
            MaxNumberOfFiles = 1
        });

        // When
        var exception = await Assert.ThrowsAsync<BucketLimitationException>(async () =>
        {
            await bucket.Sync(
            [
                new BucketSyncFile
                {
                    Path = "file1.zip",
                    Size = 1001,
                    Checksum = "file1_checksum"
                },
                new BucketSyncFile
                {
                    Path = "file2.zip",
                    Size = 1002,
                    Checksum = "file2_checksum"
                }
            ]);
        });

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.Equal("exceed_max_number_of_files", exception.Reason);
    }

    [Fact]
    public async Task fail_sync_when_exceeding_max_file_size()
    {
        // Given
        var bucket = CreateTestBucket(new BucketLimitations
        {
            IsReadOnly = false,
            ExpiredAt = DateTimeOffset.MaxValue,
            MaxBucketSize = 1024,
            MaxFileSize = 1001,
            MaxNumberOfFiles = 1024
        });

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            },
            new BucketSyncFile
            {
                Path = "file2.zip",
                Size = 1002,
                Checksum = "file2_checksum"
            }
        ]);

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.False(syncResult.IsSuccess);

        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("file2.zip", action.Path);
        Assert.Equal("file_validation", action.Action.Type);
    }

    [Fact]
    public async Task fail_sync_when_the_file_has_empty_checksum()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = ""
            }
        ]);

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.False(syncResult.IsSuccess);
        
        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("file1.zip", action.Path);
        Assert.Equal("file_validation", action.Action.Type);
    }
    
    [Fact]
    public async Task fail_sync_when_the_file_has_empty_path()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "",
                Size = 1001,
                Checksum = "file1_checksum"
            }
        ]);

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.False(syncResult.IsSuccess);
        
        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("", action.Path);
        Assert.Equal("file_validation", action.Action.Type);
    }

    [Fact]
    public async Task fail_sync_when_duplicated_file_paths_are_detected()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            },
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1002,
                Checksum = "file2_checksum"
            }
        ]);

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.False(syncResult.IsSuccess);
        
        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("file1.zip", action.Path);
        Assert.Equal("duplicated_filepath", action.Action.Type);
    }

    [Fact]
    public async Task fail_sync_when_the_requested_file_size_and_the_stored_file_size_are_different()
    {
        // Given
        var bucket = CreateTestBucket(BucketLimitations.NoLimits);

        // When
        var syncResult = await bucket.Sync(
        [
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 9999, // should be 1001
                Checksum = "file1_checksum"
            },
        ]);

        // Then
        Assert.Empty(await bucket.GetFiles());
        Assert.False(syncResult.IsSuccess);
        
        var action = Assert.Single(syncResult.RequiredActions);
        Assert.Equal("file1.zip", action.Path);
        Assert.Equal("file_validation", action.Action.Type);
    }
}