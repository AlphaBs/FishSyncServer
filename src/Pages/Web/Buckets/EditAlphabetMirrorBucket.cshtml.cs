using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class EditAlphabetMirrorBucketModel : PageModel
{
    private readonly BucketOwnerService _bucketOwnerService;
    private readonly BucketService _bucketService;
    private readonly BucketServiceFactory _bucketServiceFactory;
    
    public EditAlphabetMirrorBucketModel(
        BucketServiceFactory bucketServiceFactory, 
        BucketService bucketService, 
        BucketOwnerService bucketOwnerService)
    {
        _bucketServiceFactory = bucketServiceFactory;
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
    }

    [BindProperty] public string Id { get; set; } = string.Empty;
    [BindProperty] public string OriginUrl { get; set; } = string.Empty;
    public IAsyncEnumerable<string> Dependencies { get; set; } = AsyncEnumerable.Empty<string>();
    
    private AlphabetMirrorBucketService getService()
    {
        return (AlphabetMirrorBucketService)_bucketServiceFactory.GetRequiredService(AlphabetMirrorBucketService.AlphabetMirrorType);
    }
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var service = getService();
        
        Id = id;
        var bucket = await service.Find(id);
        if (bucket == null)
            return NotFound();
        OriginUrl = await service.GetOriginUrl(id);
        Dependencies = _bucketService.GetDependencies(id);
        
        return Page();
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
    
    public async Task<ActionResult> OnPostOriginUrlAsync(string id, string url)
    {
        if (Id != id)
            return BadRequest();
        if (string.IsNullOrEmpty(url))
            return Page();

        var service = getService();
        await service.SetOriginUrl(id, url);
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