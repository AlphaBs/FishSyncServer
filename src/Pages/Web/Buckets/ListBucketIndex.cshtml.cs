using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.Buckets;
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

    public List<BucketIndex> BucketIndexes { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync()
    {
        BucketIndexes = (await _bucketIndexService.GetAllIndexes()).ToList();
        return Page();
    }
}