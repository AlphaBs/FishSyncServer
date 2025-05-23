using System;
using System.Text.Json.Serialization;

namespace AlphabetUpdateServer.DTOs;

public class BucketIndexMetadataResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("searchable")]
    public bool Searchable { get; set; }
}
