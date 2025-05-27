using System.ComponentModel.DataAnnotations;

namespace AlphabetUpdateServer.Entities;

public class AlphabetMirrorBucketEntity : BucketEntity
{
    public const string AlphabetMirrorType = "alphabet-mirror";

    public AlphabetMirrorBucketEntity()
    {
        Type = AlphabetMirrorType;
    }
    
    [Required]
    [MaxLength(256)]
    public string Url { get; set; } = null!;
}