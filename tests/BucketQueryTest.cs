using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Repositories;

namespace AlphabetUpdateServer.Tests;

public class BucketQueryTest : BucketTestBase
{
    [Fact]
    public void GetFiles_one_common_file()
    {
        // Given
        var mockFileRepository = new Mock<IBucketFileRepository>();
        mockFileRepository
            .Setup(repo => repo.GetFiles(TestBucketId))
            .ReturnsAsync(new BucketFileEntity[] 
            {
                new BucketFileEntity
                {
                    BucketId = TestBucketId,
                    Path = "file1.zip",
                    Size = 1001,
                    LastUpdated = DateTimeOffset.MinValue,
                    Checksum = "file1_checksum"
                }
            });

        var bucket = new Bucket(
            TestBucketId,
            DateTimeOffset.MinValue,
            mockFileRepository.Object,
            TestChecksumStorage);

        // When
        var files = bucket.GetFiles()
            .ToBlockingEnumerable()
            .ToList();

        // Then
        var expectedFile = new BucketFile(
            Path: "file1.zip", 
            Size: 1001, 
            LastUpdated: DateTimeOffset.MinValue, 
            Location: "file1_location", 
            Checksum: "file1_checksum");
        Assert.Equal(new BucketFile[] { expectedFile }, files);
    }

    [Fact]
    public void GetFiles_file_cannot_find_checksum()
    {
        // Given
        var mockFileRepository = new Mock<IBucketFileRepository>();
        mockFileRepository
            .Setup(repo => repo.GetFiles(TestBucketId))
            .ReturnsAsync(new BucketFileEntity[] 
            {
                new BucketFileEntity
                {
                    BucketId = TestBucketId,
                    Path = "file1.zip",
                    Size = 1001,
                    LastUpdated = DateTimeOffset.MinValue,
                    Checksum = "cannot_find_checksum"
                }
            });

        var bucket = new Bucket(
            TestBucketId,
            DateTimeOffset.MinValue,
            mockFileRepository.Object,
            TestChecksumStorage);

        // When
        var files = bucket.GetFiles()
            .ToBlockingEnumerable()
            .ToList();

        // Then
        var expectedFile = new BucketFile(
            Path: "file1.zip", 
            Size: 1001, 
            LastUpdated: DateTimeOffset.MinValue, 
            Location: null, 
            Checksum: "cannot_find_checksum");
        Assert.Equal(new BucketFile[] { expectedFile }, files);
    }

    [Fact]
    public void GetFiles_empty()
    {
        // Given
        var mockFileRepository = new Mock<IBucketFileRepository>();
        mockFileRepository
            .Setup(repo => repo.GetFiles(TestBucketId))
            .ReturnsAsync(Enumerable.Empty<BucketFileEntity>());

        var bucket = new Bucket(
            TestBucketId,
            DateTimeOffset.MinValue,
            mockFileRepository.Object,
            TestChecksumStorage);

        // When
        var files = bucket.GetFiles()
            .ToBlockingEnumerable()
            .ToList();

        // Then
        Assert.Empty(files);
    }
}