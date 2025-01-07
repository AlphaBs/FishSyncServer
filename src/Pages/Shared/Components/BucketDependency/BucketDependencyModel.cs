namespace AlphabetUpdateServer.Pages.Shared.Components.BucketDependency;

public class BucketDependencyModel
{
    public IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();
    public bool ShowEdit { get; set; } = false;
}