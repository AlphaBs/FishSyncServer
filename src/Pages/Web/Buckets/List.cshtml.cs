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

    public ActionResult OnGetRedirectToBucketWithType(string id, string type)
    {
        switch (type)
        {
            case AlphabetMirrorBucketService.AlphabetMirrorType:
                return RedirectToPage("./ViewAlphabetMirrorBucket", new { id });
            case ChecksumStorageBucketService.ChecksumStorageType:
                return RedirectToPage("./ViewChecksumStorageBucket", new { id });
            default:
                return NotFound();
        }
    }

    public async Task<ActionResult> OnGetRedirectToBucket(string id)
    {
        var item = await _bucketService.FindBucketItem(id);
        if (item == null)
            return NotFound();
        return OnGetRedirectToBucketWithType(item.Id, item.Type);
    }
}
