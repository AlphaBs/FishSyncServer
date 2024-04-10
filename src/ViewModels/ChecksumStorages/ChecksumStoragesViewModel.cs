using AlphabetUpdateServer.Services;

namespace AlphabetUpdateServer.ViewModels.ChecksumStorages;

public class ChecksumStoragesViewModel
{
    public IReadOnlyCollection<ChecksumStorageListItem> ChecksumStorages { get; init; } = null!;
}