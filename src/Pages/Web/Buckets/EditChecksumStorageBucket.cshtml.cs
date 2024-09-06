using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class EditChecksumStorageBucketModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;

    public EditChecksumStorageBucketModel(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    public BucketLimitations Limitations { get; set; } = default!;

    [BindProperty]
    public string StorageId { get; set; } = default!;

    [BindProperty]
    public IEnumerable<BucketFile> Files { get; set; } = [];


    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        Id = id;
        Limitations = bucket.Limitations;
        StorageId = await _bucketService.GetStorageId(id);
        Files = await bucket.GetFiles();
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _bucketService.UpdateLimitationsAndStorageId(Id, Limitations, StorageId);
        return RedirectToPage("./List");
    }
}
