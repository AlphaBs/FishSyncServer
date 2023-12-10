using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Tests;

public class BucketTestBase
{
    protected readonly string TestBucketId;
    protected readonly IFileChecksumStorage TestChecksumStorage;

    public BucketTestBase()
    {
        TestBucketId = Guid.NewGuid().ToString();

        var checksumStorage = new InMemoryChecksumStorage();
        checksumStorage.Add(new FileLocation(
            checksum: "file1_checksum",
            storage: "test_storage",
            size: 1001,
            location: "file1_location"
        ));
        checksumStorage.Add(new FileLocation(
            checksum: "file2_checksum",
            storage: "test_storage",
            size: 1002,
            location: "file2_location"
        ));
        checksumStorage.SyncActionFactory = 
            checksum => new BucketSyncAction(new BucketSyncFile() { Checksum = checksum });

        TestChecksumStorage = checksumStorage;
    }
}