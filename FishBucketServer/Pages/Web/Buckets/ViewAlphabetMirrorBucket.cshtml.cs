using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using FishBucket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class ViewAlphabetMirrorBucketModel : PageModel
{
    private readonly BucketService _bucketService;
    private readonly BucketServiceFactory _bucketServiceFactory;

    public ViewAlphabetMirrorBucketModel(
        BucketService bucketService,
        BucketServiceFactory bucketServiceFactory)
    {
        _bucketService = bucketService;
        _bucketServiceFactory = bucketServiceFactory;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    public BucketUsageModel Usage { get; set; } = new();
    public IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();
    public string OriginUrl { get; set; } = default!;
    public IEnumerable<BucketFile> Files { get; set; } = [];

    private AlphabetMirrorBucketService getService()
    {
        return (AlphabetMirrorBucketService)_bucketServiceFactory.GetRequiredService(AlphabetMirrorBucketService.AlphabetMirrorType);
    }
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.Find(id);
        if (bucket == null)
        {
            return NotFound();
        }

        Id = id;
        Files = await bucket.GetFiles(CancellationToken.None);
        Dependencies = _bucketService.GetDependencies(id);
        OriginUrl = await getService().GetOriginUrl(id);

        foreach (var file in Files)
        {
            Usage.CurrentBucketSize += file.Metadata.Size;
            Usage.CurrentMaxFileSize = Math.Max(Usage.CurrentMaxFileSize, file.Metadata.Size);
            Usage.CurrentFileCount++;
        }

        Usage.ShowLimitations = false;
        return Page();
    }
}
