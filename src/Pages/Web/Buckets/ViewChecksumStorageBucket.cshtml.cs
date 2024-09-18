using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class ViewChecksumStorageBucketModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;
    private readonly BucketOwnerService _bucketOwnerService;

    public ViewChecksumStorageBucketModel(
        ChecksumStorageBucketService bucketService,
        BucketOwnerService bucketOwnerService)
    {
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    public IEnumerable<string> Owners { get; set; } = [];

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
        Owners = await _bucketOwnerService.GetOwners(id);
        return Page();
    }
}
