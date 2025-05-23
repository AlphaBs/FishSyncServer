using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using FishBucket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class ListBucketIndex : PageModel
{
    private readonly BucketIndexService _bucketIndexService;

    public ListBucketIndex(BucketIndexService bucketIndexService)
    {
        _bucketIndexService = bucketIndexService;
    }

    public IAsyncEnumerable<BucketIndex> BucketIndexes { get; set; } = AsyncEnumerable.Empty<BucketIndex>();
    
    public ActionResult OnGet()
    {
        BucketIndexes = _bucketIndexService.GetAllIndexes()
            .Where(index => index.Searchable || User.IsInRole(UserRoleNames.BucketAdmin));
        return Page();
    }
}