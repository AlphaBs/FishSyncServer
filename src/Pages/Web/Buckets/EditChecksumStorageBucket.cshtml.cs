using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class EditChecksumStorageBucketModel : PageModel
{
    private readonly BucketService _bucketService;
    private readonly BucketServiceFactory _bucketServiceFactory;
    private readonly BucketOwnerService _bucketOwnerService;
    
    public EditChecksumStorageBucketModel(
        BucketService bucketService,
        BucketServiceFactory bucketServiceFactory,
        BucketOwnerService bucketOwnerService)
    {
        _bucketService = bucketService;
        _bucketServiceFactory = bucketServiceFactory;
        _bucketOwnerService = bucketOwnerService;
    }

    [BindProperty] public string Id { get; set; } = string.Empty;
    [BindProperty] public BucketLimitations Limitations { get; set; } = new();
    [BindProperty] public string StorageId { get; set; } = string.Empty;
    public IAsyncEnumerable<string> Owners { get; set; } = AsyncEnumerable.Empty<string>();
    public IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();

    private ChecksumStorageBucketService getService()
    {
        return (ChecksumStorageBucketService)_bucketServiceFactory.GetRequiredService(ChecksumStorageBucketService.ChecksumStorageType);
    }
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var service = getService();
        
        Id = id;
        var bucket = await service.Find(id);
        if (bucket == null)
            return NotFound();
        Limitations = bucket.Limitations;
        StorageId = await service.GetStorageId(id);
        Owners = _bucketOwnerService.GetOwners(id);
        Dependencies = _bucketService.GetDependencies(id);
        
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

        var service = getService();
        await service.UpdateStorageId(id, storageId);
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
    
    public async Task<ActionResult> OnPostAddDependencyAsync(string id, string dep)
    {
        if (Id != id)
            return NotFound();
        if (string.IsNullOrEmpty(dep))
            return Page();
        
        await _bucketService.AddDependency(id, dep);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostDeleteDependencyAsync(string id, string dep)
    {
        if (Id != id)
            return NotFound();
        if (string.IsNullOrEmpty(dep))
            return Page();
        
        await _bucketService.RemoveDependency(id, dep);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostDeleteAsync(string id)
    {
        var service = getService();
        await service.Delete(id);
        return RedirectToPage("/Web/Buckets/List");
    }
}