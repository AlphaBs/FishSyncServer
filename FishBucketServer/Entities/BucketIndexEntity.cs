using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class BucketIndexEntity
{
    [Required]
    [MaxLength(64)]
    public string Id { get; set; } = default!;
    
    [MaxLength(256)]
    public string? Description { get; set; }
    
    public bool Searchable { get; set; }

    public List<BucketEntity> Buckets { get; set; } = [];
}