using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Models.Buckets.ChecksumStorages;

public class RFilesChecksumStorageConverter : IFileChecksumStorageConverter
{
    private readonly HttpClient _httpClient;

    public RFilesChecksumStorageConverter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool CanConvert(FileChecksumStorageEntity entity)
    {
        return entity.Type == RFilesChecksumStorageEntity.TypeName
            && entity is RFilesChecksumStorageEntity;
    }

    public IFileChecksumStorage ConvertFromEntityToStorage(FileChecksumStorageEntity entity)
    {
        if (entity is not RFilesChecksumStorageEntity rfilesEntity)
            throw new NotImplementedException();
        
        return new RFilesChecksumStorage(
            rfilesEntity.StorageId, 
            rfilesEntity.Host, 
            rfilesEntity.IsReadyOnly,
            _httpClient);
    }
}