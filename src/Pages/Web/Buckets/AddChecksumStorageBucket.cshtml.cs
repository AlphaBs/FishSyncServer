using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using FishBucket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class AddChecksumStorageBucketModel : PageModel
{
    private readonly BucketServiceFactory _bucketServiceFactory;

    public AddChecksumStorageBucketModel(BucketServiceFactory bucketServiceFactory)
    {
        _bucketServiceFactory = bucketServiceFactory;
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

        var service =
            (ChecksumStorageBucketService)_bucketServiceFactory.GetRequiredService(ChecksumStorageBucketService
                .ChecksumStorageType);
        await service.Create(Id, Limitations, StorageId);
        return RedirectToPage("./List");
    }
}
