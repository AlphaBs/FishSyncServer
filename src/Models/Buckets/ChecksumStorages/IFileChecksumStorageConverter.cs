using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IFileChecksumStorageConverter
{
    bool CanConvert(FileChecksumStorageEntity entity);
    IFileChecksumStorage ConvertFromEntityToStorage(FileChecksumStorageEntity entity);
}