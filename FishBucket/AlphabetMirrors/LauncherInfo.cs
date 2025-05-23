using System.Text.Json.Serialization;

namespace FishBucket.AlphabetMirrors;

public class LauncherInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("gameServerIp")]
    public string? GameServerIp { get; set; }

    [JsonPropertyName("startVersion")]
    public string? StartVersion { get; set; }

    [JsonPropertyName("startVanillaVersion")]
    public string? StartVanillaVersion { get; set; }

    [JsonPropertyName("launcherServer")]
    public string? LauncherServer { get; set; }

    [JsonPropertyName("whitelistFiles")]
    public string[] WhitelistFiles { get; set; } = [];

    [JsonPropertyName("whitelistDirs")]
    public string[] WhitelistDirs { get; set; } = [];

    [JsonPropertyName("includes")]
    public string[] IncludeFiles { get; set; } = [];
}