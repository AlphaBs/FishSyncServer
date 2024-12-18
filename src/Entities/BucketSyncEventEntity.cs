using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphabetUpdateServer.Entities;

public enum BucketSyncEventType
{
    Success,
    ActionRequired,
}

public class BucketSyncEventEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string BucketId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
    public BucketSyncEventType EventType { get; set; }
}