namespace AlphabetUpdateServer.Models;

public class BucketIndex
{
    public BucketIndex(string id) => Id = id;
    
    public string Id { get; set; }
    public string? Description { get; set; }
    public bool Searchable { get; set; }
}