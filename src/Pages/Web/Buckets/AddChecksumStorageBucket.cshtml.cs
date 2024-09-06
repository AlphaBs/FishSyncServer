using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class AddChecksumStorageBucketModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;

    public AddChecksumStorageBucketModel(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    public BucketLimitations Limitations { get; set; } = default!;

    [BindProperty]
    public string StorageId { get; set; } = default!;


    public ActionResult OnGetAsync()
    {
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _bucketService.CreateBucket(Id, Limitations, StorageId);
        return RedirectToPage("./List");
    }
}
