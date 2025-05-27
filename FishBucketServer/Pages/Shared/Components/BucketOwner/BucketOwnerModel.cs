namespace AlphabetUpdateServer.Pages.Shared.Components.BucketOwner;

public class BucketOwnerModel
{
    public IAsyncEnumerable<string> Owners { get; set; } = AsyncEnumerable.Empty<string>();
    public bool ShowEdit { get; set; } = false;
}