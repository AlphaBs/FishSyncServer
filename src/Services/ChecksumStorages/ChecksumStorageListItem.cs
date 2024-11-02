namespace AlphabetUpdateServer.Services.ChecksumStorages;

public record ChecksumStorageListItem
(
    string Id,
    string Type,
    bool IsReadonly
);