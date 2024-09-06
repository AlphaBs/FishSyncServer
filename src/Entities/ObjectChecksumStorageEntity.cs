namespace AlphabetUpdateServer.Entities;

public class ObjectChecksumStorageEntity : ChecksumStorageEntity
{
    public static readonly string ObjectType = "object";

    public ObjectChecksumStorageEntity()
    {
        Type = ObjectType;
    }

    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public string ServiceEndpoint { get; set; } = null!;
    public string PublicEndpoint { get; set; } = null!;
}