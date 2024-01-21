namespace AlphabetUpdateServer.Models.Buckets;

public class FileLocation
{
    public FileLocation(
        string storage, 
        string location, 
        BucketFileMetadata metadata) =>
        (Storage, Location, Metadata) = (storage, location, metadata);

    public string Storage { get; private set; }
    public string Location { get; private set; }
    public BucketFileMetadata Metadata { get; private set; }
}