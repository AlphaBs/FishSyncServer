namespace AlphabetUpdateServer.Models;

public record SyncAction
(
    string Type,
    IReadOnlyDictionary<string, string>? Parameters
);