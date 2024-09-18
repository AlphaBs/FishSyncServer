using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class EditChecksumStorageBucketModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;
    private readonly BucketOwnerService _bucketOwnerService;
    
    public EditChecksumStorageBucketModel(
        ChecksumStorageBucketService bucketService,
        BucketOwnerService bucketOwnerService)
    {
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
    }

    [BindProperty] public string Id { get; set; } = string.Empty;

    [BindProperty] public BucketLimitations Limitations { get; set; } = new();

    [BindProperty] public string StorageId { get; set; } = string.Empty;

    [BindProperty] public IEnumerable<string> Owners { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        Id = id;
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
            return NotFound();
        Limitations = bucket.Limitations;
        StorageId = await _bucketService.GetStorageId(id);
        Owners = await _bucketOwnerService.GetOwners(id);
        
        return Page();
    }

    public async Task<ActionResult> OnPostLimitationsAsync(string id)
    {
        if (Id != id)
            return BadRequest();

        await _bucketService.UpdateLimitations(id, Limitations);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostStorageIdAsync(string id, string storageId)
    {
        if (Id != id)
            return BadRequest();
        if (string.IsNullOrEmpty(storageId))
            return Page();
            
        await _bucketService.UpdateStorageId(id, storageId);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostAddOwnerAsync(string id, string username)
    {
        if (Id != id)
            return NotFound();
        if (string.IsNullOrEmpty(username))
            return Page();
        
        await _bucketOwnerService.AddOwner(id, username);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostDeleteOwnerAsync(string id, string username)
    {
        if (Id != id)
            return NotFound();
        if (string.IsNullOrEmpty(username))
            return Page();
        
        await _bucketOwnerService.RemoveOwner(id, username);
        return RedirectToPage();
    }
}