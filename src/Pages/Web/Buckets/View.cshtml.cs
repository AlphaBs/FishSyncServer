using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class View : PageModel
{
    private readonly BucketService _bucketService;

    public View(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    public async Task<ActionResult> OnGetAsync(string id, string? type)
    {
        if (string.IsNullOrEmpty(type))
            return await OnGetRedirectToBucketAsync(id);
        else
            return OnGetRedirectToBucketWithType(id, type);
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

    public async Task<ActionResult> OnGetRedirectToBucketAsync(string id)
    {
        var item = await _bucketService.FindBucketItem(id);
        if (item == null)
            return NotFound();
        return OnGetRedirectToBucketWithType(item.Id, item.Type);
    }
}