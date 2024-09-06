using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.Buckets;

public class ListModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;

    public ListModel(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [BindProperty]
    public IEnumerable<BucketListItem> Buckets { get; set; } = [];

    public async Task<ActionResult> OnGetAsync()
    {
        Buckets = await _bucketService.GetAllBuckets();
        return Page();
    }
}
