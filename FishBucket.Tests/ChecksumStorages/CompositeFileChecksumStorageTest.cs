using FishBucket.ChecksumStorages.Storages;

namespace FishBucket.Tests.ChecksumStorages;

public class CompositeFileChecksumStorageTest
{
    private string TestChecksum = Guid.NewGuid().ToString();

    [Fact]
    public void readonly_when_no_storage()
    {
        var storage = new CompositeChecksumStorage();
        Assert.True(storage.IsReadOnly);
    }

    [Fact]
    public void readonly_when_all_storage_is_readonly()
    {
        var storage = new CompositeChecksumStorage();
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: true));

        Assert.True(storage.IsReadOnly);
    }

    [Fact]
    public void writeable_when_has_writeable_storage()
    {
        var storage = new CompositeChecksumStorage();
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: false));

        Assert.False(storage.IsReadOnly);
    }

    [Fact]
    public async Task sync_throws_when_no_storage()
    {
        var storage = new CompositeChecksumStorage();
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await storage.Sync([TestChecksum]);
        });
    }

    [Fact]
    public async Task sync_throws_when_no_writeable_storage()
    {
        var storage = new CompositeChecksumStorage();
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: true));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await storage.Sync([TestChecksum]);
        });
    }

    [Fact]
    public async Task GetAllFiles_empty()
    {
        // Given
        var storage = new CompositeChecksumStorage();

        // When
        var files = await storage.GetAllFiles();

        // Then
        Assert.Empty(files);
    }

    [Fact]
    public async Task GetAllFiles()
    {
        // Given
        var testFiles = new[]
        {
            createChecksumStorageFile("0", "0"),
            createChecksumStorageFile("1", "1"),
            createChecksumStorageFile("2", "2"),
            createChecksumStorageFile("3", "3")
        };
        var storage = new CompositeChecksumStorage();
        var inner1 = createMockStorage(true);
        inner1.Add(testFiles[0]);
        inner1.Add(testFiles[1]);
        storage.AddStorage(inner1);
        var inner2 = createMockStorage( true);
        inner2.Add(testFiles[2]);
        inner2.Add(testFiles[3]);
        storage.AddStorage(inner2);

        // When
        var actualFiles = await storage.GetAllFiles();

        // Then
        Assert.Equal(testFiles, actualFiles);
    }

    [Fact]
    public async Task Query()
    {
        // Given
        var testFiles = new[]
        {
            createChecksumStorageFile("0", "0"),
            createChecksumStorageFile("1", "1"),
            createChecksumStorageFile("2", "2"),
            createChecksumStorageFile("3", "3")
        };
        var storage = new CompositeChecksumStorage();
        var inner1 = createMockStorage(true);
        inner1.Add(testFiles[0]);
        inner1.Add(testFiles[1]);
        storage.AddStorage(inner1);
        var inner2 = createMockStorage(true);
        inner2.Add(testFiles[2]);
        inner2.Add(testFiles[3]);
        storage.AddStorage(inner2);

        // When
        var queryChecksums = new[]
        {
            "a", "1", "3", "6", "9"
        };

        var queryResult = await storage.Query(queryChecksums);

        // Then
        Assert.Equal([testFiles[1], testFiles[3]], queryResult.FoundFiles);
        Assert.Equal(["a", "6", "9"], queryResult.NotFoundChecksums);
    }

    private InMemoryChecksumStorage createMockStorage(bool isReadOnly)
    {
        return new InMemoryChecksumStorage()
        {
            IsReadOnly = isReadOnly
        };
    }

    private ChecksumStorageFile createChecksumStorageFile(string checksum, string location)
    {
        return new ChecksumStorageFile
        (
            Checksum: checksum,
            Location: location,
            Metadata: new FileMetadata
            (
                Size: 1,
                LastUpdated: DateTimeOffset.MinValue,
                Checksum: location
            )
        );
    }
}