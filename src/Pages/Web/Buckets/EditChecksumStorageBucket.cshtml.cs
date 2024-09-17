using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class EditChecksumStorageBucketModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;

    public EditChecksumStorageBucketModel(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [BindProperty] public string Id { get; set; } = string.Empty;

    [BindProperty] public BucketLimitations Limitations { get; set; } = new();

    [BindProperty] public string StorageId { get; set; } = string.Empty;
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var storageId = await _bucketService.GetStorageId(id);

        Id = id;
        Limitations = bucket.Limitations;
        StorageId = storageId;

        return Page();
    }

    public async Task<ActionResult> OnPostAsync(string id)
    {
        if (Id != id)
        {
            return BadRequest();
        }

        await _bucketService.UpdateLimitationsAndStorageId(id, Limitations, StorageId);
        return RedirectToPage();
    }
}