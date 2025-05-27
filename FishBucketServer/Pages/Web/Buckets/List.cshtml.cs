using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class ListModel : PageModel
{
    private readonly BucketService _bucketService;

    public ListModel(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [BindProperty]
    public IAsyncEnumerable<BucketListItem> Buckets { get; set; } = AsyncEnumerable.Empty<BucketListItem>();

    public ActionResult OnGet()
    {
        Buckets = _bucketService.GetAllBuckets();
        return Page();
    }
}
