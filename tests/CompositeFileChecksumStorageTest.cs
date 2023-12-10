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
    public void IsReadOnly_returns_true_when_no_storage()
    {
        var storage = new CompositeFileChecksumStorage();
        Assert.True(storage.IsReadOnly);
    }

    [Fact]
    public void IsReadOnly_returns_true_all_storage_is_readonly()
    {
        // Given
        var storage = new CompositeFileChecksumStorage();
        storage.AddStorage(createMockStorage(actionType: "1", isReadOnly: true));
        storage.AddStorage(createMockStorage(actionType: "2", isReadOnly: true));

        // When
        var isReadOnly = storage.IsReadOnly;

        // Then
        Assert.True(isReadOnly);
    }

    [Fact]
    public void IsReadOnly_returns_false_when_has_non_readonly_storage()
    {
        // Given
        var storage = new CompositeFileChecksumStorage();
        storage.AddStorage(createMockStorage(actionType: "1", isReadOnly: true));
        storage.AddStorage(createMockStorage(actionType: "2", isReadOnly: false));

        // When
        var isReadOnly = storage.IsReadOnly;

        // Then
        Assert.False(isReadOnly);
    }

    [Fact]
    public void CreateSyncAction_return_first()
    {
        // Given
        var storage = new CompositeFileChecksumStorage();
        storage.AddStorage(createMockStorage(actionType: "1", isReadOnly: true));
        storage.AddStorage(createMockStorage(actionType: "2", isReadOnly: false));
        storage.AddStorage(createMockStorage(actionType: "3", isReadOnly: true));

        // When
        var action = storage.CreateSyncAction(TestChecksum);

        // Then
        Assert.Equal("2", action.ActionType);
    }

    [Fact]
    public void CreateSyncAction_no_storage()
    {
        var storage = new CompositeFileChecksumStorage();
        Assert.Throws<InvalidOperationException>(() =>
        {
            storage.CreateSyncAction(TestChecksum);
        });
    }

    [Fact]
    public void CreateSyncAction_no_available_storage()
    {
        var storage = new CompositeFileChecksumStorage();
        storage.AddStorage(createMockStorage(actionType: "1", isReadOnly: true));
        storage.AddStorage(createMockStorage(actionType: "2", isReadOnly: true));
        storage.AddStorage(createMockStorage(actionType: "3", isReadOnly: true));

        Assert.Throws<InvalidOperationException>(() =>
        {
            storage.CreateSyncAction(TestChecksum);
        });
    }

    [Fact]
    public void GetAllFiles_empty()
    {
        // Given
        var storage = new CompositeFileChecksumStorage();

        // When
        var files = storage.GetAllFiles().ToBlockingEnumerable();

        // Then
        Assert.Empty(files);
    }

    [Fact]
    public void GetAllFiles()
    {
        // Given
        var testFiles = new FileLocation[]
        {
            new FileLocation("0", "0", 0, "0"),
            new FileLocation("1", "1", 1, "1"),
            new FileLocation("2", "2", 2, "2"),
            new FileLocation("3", "3", 3, "3"),
        };
        var storage = new CompositeFileChecksumStorage();
        var inner1 = createMockStorage("a", true);
        inner1.Add(testFiles[0]);
        inner1.Add(testFiles[1]);
        storage.AddStorage(inner1);
        var inner2 = createMockStorage("b", true);
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
        var testFiles = new FileLocation[]
        {
            new FileLocation("0", "0", 0, "0"),
            new FileLocation("1", "1", 1, "1"),
            new FileLocation("2", "2", 2, "2"),
            new FileLocation("3", "3", 3, "3"),
        };
        var storage = new CompositeFileChecksumStorage();
        var inner1 = createMockStorage("a", true);
        inner1.Add(testFiles[0]);
        inner1.Add(testFiles[1]);
        storage.AddStorage(inner1);
        var inner2 = createMockStorage("b", true);
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
        var expectedFiles = new FileLocation[]
        {
            testFiles[1], testFiles[3]
        };
        Assert.Equal(expectedFiles, actualFiles);
    }

    // 어떤 storage 에서 반환했는지 알기 위해서 storage 마다 다른 ActionType 을 가짐
    private InMemoryChecksumStorage createMockStorage(string actionType, bool isReadOnly)
    {
        var storage = new InMemoryChecksumStorage();
        storage.SyncActionFactory = checksum =>
        {
            var action = new BucketSyncAction(new BucketSyncFile { Checksum = checksum })
            {
                ActionType = actionType
            };
            return action;
        };
        storage.IsReadOnly = isReadOnly;
        return storage;
    }
}