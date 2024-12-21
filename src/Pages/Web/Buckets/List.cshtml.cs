using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class ListModel : PageModel
{
    private readonly BucketService _bucketService;

    public ListModel(BucketService bucketService)
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
