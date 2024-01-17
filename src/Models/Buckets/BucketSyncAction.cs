namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncAction
{
    public BucketSyncFile? File { get; set; }
    public string? ActionType { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
}