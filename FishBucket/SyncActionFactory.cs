namespace FishBucket;

public class SyncActionFactory
{
    public static SyncAction CreateHttpRequest(string method, string url, Dictionary<string, string> headers)
    {
        var parameters = new Dictionary<string, string>
        {
            ["method"] = method,
            ["url"] = url,
        };

        foreach (var kv in headers)
        {
            parameters["headers." + kv.Key] = kv.Value;
        }

        return new SyncAction
        (
            Type: "Http",
            Parameters: parameters
        );
    }
}