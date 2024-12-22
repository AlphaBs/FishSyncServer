using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class EditAlphabetMirrorBucketModel : PageModel
{
    private readonly BucketServiceFactory _bucketServiceFactory;
    
    public EditAlphabetMirrorBucketModel(BucketServiceFactory bucketServiceFactory)
    {
        _bucketServiceFactory = bucketServiceFactory;
    }

    [BindProperty] public string Id { get; set; } = string.Empty;
    [BindProperty] public string OriginUrl { get; set; } = string.Empty;

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
        
        return Page();
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
}