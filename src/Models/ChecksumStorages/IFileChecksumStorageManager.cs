namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IFileChecksumStorageManager
{
    ValueTask<IFileChecksumStorage> GetStorageForBucket(string bucketId);
}