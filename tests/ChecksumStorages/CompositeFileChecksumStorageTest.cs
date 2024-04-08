using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Tests;

public class CompositeFileChecksumStorageTest
{
    private string TestChecksum;

    public CompositeFileChecksumStorageTest()
    {
        TestChecksum = Guid.NewGuid().ToString();
    }

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
    public void CreateSyncAction_return_first_writeable_storage()
    {
        // Given
        var storage = new CompositeChecksumStorage();
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: false));
        storage.AddStorage(createMockStorage(isReadOnly: true));

        // When
        var action = storage.CreateSyncAction(TestChecksum);

        // Then
        Assert.Equal(TestChecksum, action.Type);
    }

    [Fact]
    public void CreateSyncAction_throws_when_no_storage()
    {
        var storage = new CompositeChecksumStorage();
        Assert.Throws<InvalidOperationException>(() =>
        {
            storage.CreateSyncAction(TestChecksum);
        });
    }

    [Fact]
    public void CreateSyncAction_throws_when_no_writeable_storage()
    {
        var storage = new CompositeChecksumStorage();
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: true));
        storage.AddStorage(createMockStorage(isReadOnly: true));

        Assert.Throws<InvalidOperationException>(() =>
        {
            storage.CreateSyncAction(TestChecksum);
        });
    }

    [Fact]
    public void GetAllFiles_empty()
    {
        // Given
        var storage = new CompositeChecksumStorage();

        // When
        var files = storage.GetAllFiles().ToBlockingEnumerable();

        // Then
        Assert.Empty(files);
    }

    [Fact]
    public void GetAllFiles()
    {
        // Given
        var testFiles = new ChecksumStorageFile[]
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
        var actualFiles = storage.GetAllFiles().ToBlockingEnumerable();

        // Then
        Assert.Equal(testFiles, actualFiles);
    }

    [Fact]
    public void Query()
    {
        // Given
        var testFiles = new ChecksumStorageFile[]
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
        var queryChecksums = new string[]
        {
            "a", "1", "3", "6", "9"
        };

        var actualFiles = storage
            .Query(queryChecksums)
            .ToBlockingEnumerable();

        // Then
        var expectedFiles = new ChecksumStorageFile[]
        {
            testFiles[1], testFiles[3]
        };
        Assert.Equal(expectedFiles, actualFiles);
    }

    private InMemoryChecksumStorage createMockStorage(bool isReadOnly)
    {
        return new InMemoryChecksumStorage()
        {
            IsReadOnly = isReadOnly,
            SyncActionFactory = checksum => new SyncAction(checksum, null)
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