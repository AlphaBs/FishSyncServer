namespace AlphabetUpdateServer.Pages.Shared.Components.BucketOwner;

public class BucketOwnerModel
{
    public IEnumerable<string> Owners { get; set; } = [];
    public bool ShowEdit { get; set; } = false;
}