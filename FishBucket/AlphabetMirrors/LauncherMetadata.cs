using System.Text.Json.Serialization;

namespace FishBucket.AlphabetMirrors;

public class LauncherMetadata
{
    [JsonPropertyName("lastInfoUpdate")]
    public DateTime LastInfoUpdate { get; set; }

    [JsonPropertyName("launcher")]
    public LauncherInfo? Launcher { get; set; }

    [JsonPropertyName("files")]
    public UpdateFileCollection? Files { get; set; }
}