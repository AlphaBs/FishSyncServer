namespace AlphabetUpdateServer.Pages.Shared.Components.BucketDependency;

public class BucketDependencyModel
{
    public IEnumerable<string> Dependencies { get; set; } = [];
    public bool ShowEdit { get; set; } = false;
}