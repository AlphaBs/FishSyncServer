namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IFileChecksumStorageFactory
{
    ValueTask<IFileChecksumStorage> CreateStorageForBucket(string bucketId);
}