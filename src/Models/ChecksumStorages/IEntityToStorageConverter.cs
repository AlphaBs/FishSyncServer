using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Models.ChecksumStorages;

public interface IEntityToStorageConverter
{
    bool CanCreate(FileChecksumStorageEntity entity);
    IFileChecksumStorage Create(FileChecksumStorageEntity entity);   
}