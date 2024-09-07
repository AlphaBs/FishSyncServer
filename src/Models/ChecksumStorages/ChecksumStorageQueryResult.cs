namespace AlphabetUpdateServer.Models.ChecksumStorages;

public class ChecksumStorageQueryResult
{
    public IReadOnlyCollection<string> NotFoundChecksums { get; }
    public IReadOnlyCollection<ChecksumStorageFile> FoundFiles { get; }

    public ChecksumStorageQueryResult(
        IReadOnlyCollection<ChecksumStorageFile> foundFiles,
        IReadOnlyCollection<string> notFoundChecksums)
    {
        NotFoundChecksums = notFoundChecksums;
        FoundFiles = foundFiles;
    }
}