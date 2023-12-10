using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;

namespace AlphabetUpdateServer.Tests;

public class BucketSyncTest : BucketTestBase
{
    [Fact]
    public async Task Sync_success()
    {
        // Given
        var files = new BucketSyncFile[]
        {
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            }
        };

        // When
        var (result, entities) = await BucketSyncProcessor.Sync(TestBucketId, TestChecksumStorage, files);

        // Then
        var addedFile = new BucketFileEntity
        {
            BucketId = TestBucketId,
            Path = "file1.zip",
            Size = 1001,
            Checksum = "file1_checksum",
            LastUpdated = result.UpdatedAt
        };
        Assert.True(result.IsSuccess);
        Assert.Equal(new BucketFileEntity[] { addedFile }, entities);
    }

    [Fact]
    public async Task Sync_action_required()
    {
        // Given
        var files = new BucketSyncFile[]
        {
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 1001,
                Checksum = "not_existing_file"
            }
        };

        // When
        var (result, entities) = await BucketSyncProcessor.Sync(TestBucketId, TestChecksumStorage, files);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Empty(entities);
        Assert.Equal("not_existing_file", result.RequiredActions.First().File.Checksum);
    }

    [Fact]
    public async void Sync_invalid_size()
    {
        // Given
        var files = new BucketSyncFile[]
        {
            new BucketSyncFile
            {
                Path = "file1.zip",
                Size = 9999,
                Checksum = "file1_checksum"
            }
        };

        // When
        var (result, entities) = await BucketSyncProcessor.Sync(TestBucketId, TestChecksumStorage, files);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Empty(entities);
        Assert.Equal("file1_checksum", result.RequiredActions.First().File.Checksum);
        Assert.Equal(BucketSyncActionTypes.Validation, result.RequiredActions.First().ActionType);
    }

    [Fact]
    public async void Sync_duplicated_paths()
    {
        // Given
        var files = new BucketSyncFile[]
        {
            new BucketSyncFile
            {
                Path = "path.zip",
                Size = 1001,
                Checksum = "file1_checksum"
            },
            new BucketSyncFile
            {
                Path = "path.zip",
                Size = 1002,
                Checksum = "file2_checksum"
            }
        };

        // When
        var (result, entities) = await BucketSyncProcessor.Sync(TestBucketId, TestChecksumStorage, files);

        // Then
        Assert.False(result.IsSuccess);
        Assert.Empty(entities);
        Assert.Contains("path.zip", result.RequiredActions.First().File.Path);
        Assert.Equal(BucketSyncActionTypes.DuplicatedFilePath, result.RequiredActions.First().ActionType);
    }
}