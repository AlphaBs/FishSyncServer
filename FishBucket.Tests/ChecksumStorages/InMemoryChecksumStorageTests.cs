using FishBucket.ChecksumStorages.Storages;

namespace FishBucket.Tests.ChecksumStorages;

public class InMemoryChecksumStorageTests
{
    [Fact]
    public async Task get_all_files_returns_empty()
    {
        var storage = new InMemoryChecksumStorage();
        var files = await storage.GetAllFiles();
        Assert.Empty(files);
    }

    [Fact]
    public async Task get_all_files_returns_expected()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new []
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);
        
        var result = await storage.GetAllFiles();
        Assert.Equal(files, result);
    }
    
    [Fact]
    public async Task query_files()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new []
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);

        var query = new []{"z", "1", "x", "2", "c"};
        var result = await storage.Query(query);
        Assert.Equal([files[0], files[1]], result.FoundFiles);
        Assert.Equal(["z","x","c"], result.NotFoundChecksums);
    }

    [Fact]
    public async Task query_duplicated_checksums()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new []
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);

        var query = new[] { "1", "1", "2", "2", "a", "a" };
        var result = await storage.Query(query);
        Assert.Equal([files[0], files[1]], result.FoundFiles);
        Assert.Equal(["a"], result.NotFoundChecksums);
    }
    
    [Fact]
    public async Task query_files_found_all()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new []
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);

        var query = new []{"1","2","3"};
        var result = await storage.Query(query);
        Assert.Equal(files, result.FoundFiles);
        Assert.Empty(result.NotFoundChecksums);
    }

    [Fact]
    public async Task query_files_found_nothing()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new []
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);

        var query = new []{"a", "b"};
        var result = await storage.Query(query);
        Assert.Empty(result.FoundFiles);
        Assert.Equal(["a", "b"], result.NotFoundChecksums);
    }

    [Fact]
    public async Task sync_found_and_not_found_checksums()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new[]
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);

        var result = await storage.Sync(["z", "1", "x", "2", "c"]);
        Assert.Equal([files[0], files[1]], result.SuccessFiles);
        Assert.Equal(3, result.RequiredActions.Count);
        Assert.Contains(result.RequiredActions, action => action.Checksum == "z");
        Assert.Contains(result.RequiredActions, action => action.Checksum == "x");
        Assert.Contains(result.RequiredActions, action => action.Checksum == "c");
    }
    
    [Fact]
    public async Task sync_nothing()
    {
        var storage = new InMemoryChecksumStorage();
        var result = await storage.Sync([]);
        Assert.Empty(result.SuccessFiles);
        Assert.Empty(result.RequiredActions);
    }

    [Fact]
    public async Task sync_duplicated_checksums()
    {
        var storage = new InMemoryChecksumStorage();
        var files = new[]
        {
            createFile("1", "a"),
            createFile("2", "b"),
            createFile("3", "c"),
        };
        storage.AddRange(files);

        var result = await storage.Sync(["a", "1", "a", "1"]);
        Assert.Equal([files[0]], result.SuccessFiles);
        Assert.Equal(1, result.RequiredActions.Count);
        Assert.Contains(result.RequiredActions, action => action.Checksum == "a");
    }

    private static ChecksumStorageFile createFile(string checksum, string location)
    {
        return new ChecksumStorageFile(checksum, location, new FileMetadata(1, DateTimeOffset.MinValue, checksum));
    }
}