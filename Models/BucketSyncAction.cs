namespace AlphabetUpdateServer.Models;

public class BucketSyncAction
{
    public BucketSyncAction(BucketSyncFile file) => 
        File = file;

    public BucketSyncFile File { get; }
    public string? ActionType { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
}